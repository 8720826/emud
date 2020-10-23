using AutoMapper;
using Emprise.Domain.Core.Authorization;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.CommandHandlers;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Models.Chat;
using Emprise.Domain.Core.Notifications;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.Player.Models;
using Emprise.Domain.Player.Services;
using Emprise.Domain.PlayerRelation.Services;
using Emprise.Domain.Room.Services;
using Emprise.Infra.Authorization;
using Emprise.MudServer.Commands;
using Emprise.MudServer.Events;
using Emprise.MudServer.Hubs.Models;
using Emprise.MudServer.Models;
using Emprise.MudServer.Queues;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emprise.MudServer.CommandHandlers
{

    public class PlayerCommandHandler : CommandHandler,
        IRequestHandler<CreateCommand, Unit>,
        IRequestHandler<JoinGameCommand, Unit>,
        IRequestHandler<InitGameCommand, Unit>,
        IRequestHandler<MoveCommand, Unit>,
        IRequestHandler<SearchCommand, Unit>,
        IRequestHandler<ShowPlayerCommand, Unit>,
        IRequestHandler<ShowMeCommand, Unit>,
        IRequestHandler<ShowMyStatusCommand, Unit>,
        IRequestHandler<PingCommand, Unit>,
        IRequestHandler<SendMessageCommand, Unit>


    {
        private readonly IMediatorHandler _bus;
        private readonly ILogger<PlayerCommandHandler> _logger;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly IMapper _mapper;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly IRoomDomainService _roomDomainService;
        private readonly IAccountContext _account;
        private readonly IDelayedQueue _delayedQueue;
        private readonly IRecurringQueue _recurringQueue;
        private readonly IMudProvider _mudProvider;
        private readonly AppConfig _appConfig;
        private readonly IRedisDb _redisDb;
        private readonly IMemoryCache _cache;
        private readonly IMudOnlineProvider _mudOnlineProvider;
        private readonly IQueueHandler _queueHandler;
        private readonly IPlayerRelationDomainService _playerRelationDomainService;



        public PlayerCommandHandler(
            IMediatorHandler bus,
            ILogger<PlayerCommandHandler> logger,
            IHttpContextAccessor httpAccessor,
            IMapper mapper,
            IPlayerDomainService playerDomainService,
            IRoomDomainService roomDomainService,
            IAccountContext account,
            IDelayedQueue delayedQueue,
            IRecurringQueue recurringQueue,
            IMudProvider mudProvider,
            IOptionsMonitor<AppConfig> appConfig,
            IRedisDb redisDb,
            IMemoryCache cache,
            INotificationHandler<DomainNotification> notifications,
            IMudOnlineProvider mudOnlineProvider,
            IQueueHandler queueHandler,
            IPlayerRelationDomainService playerRelationDomainService,
            IUnitOfWork uow) : base(uow, bus, notifications)
        {

            _cache = cache;
            _bus = bus;
            _logger = logger;
            _httpAccessor = httpAccessor;
            _mapper = mapper;
            _playerDomainService = playerDomainService;
            _roomDomainService = roomDomainService;
            _account = account;
            _delayedQueue = delayedQueue;
            _recurringQueue = recurringQueue;
            _mudProvider = mudProvider;
            _appConfig = appConfig.CurrentValue;
            _redisDb = redisDb;
            _mudOnlineProvider = mudOnlineProvider;
            _queueHandler = queueHandler;
            _playerRelationDomainService = playerRelationDomainService;
        }

        public async Task<Unit> Handle(CreateCommand command, CancellationToken cancellationToken)
        {
            var name = command.Name;
            var gender = command.Gender;
            var userId = command.UserId;
            var str = command.Str;
            var @int = command.Int;
            var dex = command.Dex;
            var con = command.Con;

            var player = await _playerDomainService.Get(p => p.Name == name);

            if (player != null)
            {
                await _bus.RaiseEvent(new DomainNotification("角色名已被使用，请更改！"));
                return Unit.Value;
            }

            player = await _playerDomainService.Get(x => x.UserId == userId);
            if (player != null)
            {
                await _bus.RaiseEvent(new DomainNotification("已经超过最大可创建角色数！"));
                return Unit.Value;
            }

            if (str + @int + dex + con != 80)
            {
                await _bus.RaiseEvent(new DomainNotification("所有先天属性之和必须为80！"));
                return Unit.Value;
            }

            var roomId = _appConfig.Site.BornRoomId;
            if (roomId <= 0)
            {
                await _bus.RaiseEvent(new DomainNotification("未设置出生地点！"));
                return Unit.Value;
            }

            var room = await _roomDomainService.Get(roomId);
            if (room == null)
            {
                await _bus.RaiseEvent(new DomainNotification("设置的出生地点不存在！"));
                return Unit.Value;
            }

            Random random = new Random();

            player = new PlayerEntity
            {
                CreateDate = DateTime.Now,
                LastDate = DateTime.Now,
                Level = 1,
                Name = name,
                UserId = userId,
                Status = PlayerStatusEnum.空闲,
                Gender = gender,
                Age = 14 * 12,
                ConAdd = 0,
                DexAdd = 0,
                FactionId = 0,
                IntAdd = 0,
                Money = 0,
                RoomId = roomId,
                Title = "",
                StrAdd = 0,
                Atk = 0,
                Str = str,
                Con = con,
                Int = @int,
                Dex = dex,
                Exp = 0,
                Cor = 20,
                Cps = 20,


                Pot = 0,
                Kar = random.Next(1, 100),
                Def = 0,
                Hp = 0,
                LimitMp = 0,
                MaxHp = 0,
                MaxMp = 0,
                Mp = 0,

                Hit = 0,
                Parry = 0,
                Flee = 0,
                Per = random.Next(10, 50),
                Nrg = 0
            };

            player.Computed();

            await _playerDomainService.Add(player);

            var jwtAccount = new JwtAccount
            {
                UserId = userId,
                Email = _account.Email,
                PlayerId = player.Id,
                PlayerName = player.Name
            };

            await _httpAccessor.HttpContext.SignIn("user", jwtAccount);

            if (await Commit())
            {
                await _bus.RaiseEvent(new CreatedEvent(player)).ConfigureAwait(false);
            }


            return Unit.Value;
        }

        public async Task<Unit> Handle(JoinGameCommand command, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Handle JoinGameCommand:{JsonConvert.SerializeObject(command)}");
            var userId = command.UserId;
            var playerId = command.PlayerId;

            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            if (player.UserId != userId)
            {
                await _bus.RaiseEvent(new DomainNotification("该角色不存在！"));
                return Unit.Value;
            }

            if (_account == null)
            {
                await _bus.RaiseEvent(new DomainNotification("未登录！"));
                return Unit.Value;
            }

            var jwtAccount = new JwtAccount
            {
                UserId = userId,
                Email = _account.Email,
                PlayerId = playerId,
                PlayerName = player.Name
            };

            await _httpAccessor.HttpContext.SignIn("user", jwtAccount);

            if (await Commit())
            {
                await _bus.RaiseEvent(new JoinedGameEvent(player)).ConfigureAwait(false);
            }

            return Unit.Value;
        }

        public async Task<Unit> Handle(InitGameCommand command, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Handle InitGameCommand:{JsonConvert.SerializeObject(command)}");

            var playerId = command.PlayerId;

            if (playerId <= 0)
            {
                await _bus.RaiseEvent(new DomainNotification($"请重新进入！"));
                return Unit.Value;
            }

            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            var room = await _roomDomainService.Get(player.RoomId);
            if (room == null)
            {
                await _bus.RaiseEvent(new DomainNotification("房间不存在！"));
                return Unit.Value;
            }

            player.LastDate = DateTime.Now;


            await _cache.GetOrCreateAsync(CacheKey.IsActivityIn24Hours, async x => {
                x.AbsoluteExpiration = DateTime.UtcNow.AddHours(24);
                Random random = new Random();
                player.Kar = random.Next(1, 100);
                return await Task.FromResult(true);
            });

            player.Computed();


            await _playerDomainService.Update(player);



            if (await Commit())
            {
                await _bus.RaiseEvent(new InitGameEvent(player)).ConfigureAwait(false);
                await _bus.RaiseEvent(new PlayerInRoomEvent(player, room)).ConfigureAwait(false);
            }


            return Unit.Value;
        }

        public async Task<Unit> Handle(MoveCommand command, CancellationToken cancellationToken)
        {

            var roomId = command.RoomId;
            var playerId = command.PlayerId;

            if (roomId <= 0)
            {
                await _bus.RaiseEvent(new DomainNotification($"房间不存在！"));
                return Unit.Value;
            }

            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }
            if (player.Status != PlayerStatusEnum.空闲)
            {
                await _bus.RaiseEvent(new DomainNotification($"请先停止{player.Status}！"));
                return Unit.Value;
            }
            var oldRoomId = player.RoomId;
            if (oldRoomId == roomId)
            {
                await _bus.RaiseEvent(new DomainNotification("你已在此地！请刷新或重新进入游戏！"));
                return Unit.Value;
            }
            var oldRoom = await _roomDomainService.Get(oldRoomId);
            if (oldRoom != null)
            {
                if (!new[] { oldRoom.West, oldRoom.East, oldRoom.North, oldRoom.South }.Contains(roomId))
                {
                    await _bus.RaiseEvent(new DomainNotification($"无法移动到该房间！"));
                    return Unit.Value;
                }
            }

            var newRoom = await _roomDomainService.Get(roomId);
            if (newRoom == null)
            {
                await _bus.RaiseEvent(new DomainNotification("房间不存在！"));
                return Unit.Value;
            }

            player.RoomId = roomId;
            await _playerDomainService.Update(player);



            if (await Commit())
            {
                await _bus.RaiseEvent(new MovedEvent(player, newRoom, oldRoom));
            }


            return Unit.Value;
        }

        public async Task<Unit> Handle(SearchCommand command, CancellationToken cancellationToken)
        {
            var playerId = command.PlayerId;

            await _mudProvider.ShowMessage(playerId, "你什么也没发现。。。");

            if (await Commit())
            {
                //新手任务
                var searchTimes = await _redisDb.StringGet<int>(string.Format(RedisKey.SearchTimes, playerId));
                if (searchTimes <= 0)
                {
                    await _queueHandler.SendQueueMessage(new CompleteQuestNewbieQuestQueue(playerId, NewbieQuestEnum.第一次探索));
                }
                searchTimes++;
                await _redisDb.StringSet(string.Format(RedisKey.SearchTimes, playerId), searchTimes);
            }
            return Unit.Value;
        }


        public async Task<Unit> Handle(ShowMeCommand command, CancellationToken cancellationToken)
        {
            var playerId = command.PlayerId;
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                return Unit.Value;
            }
            var myInfo = _mapper.Map<MyInfo>(player);

            await _mudProvider.ShowMe(playerId, myInfo);
            return Unit.Value;
        }

        public async Task<Unit> Handle(ShowMyStatusCommand command, CancellationToken cancellationToken)
        {
            var playerId = command.PlayerId;
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                return Unit.Value;
            }
            var myInfo = _mapper.Map<MyInfo>(player);

            await _mudProvider.ShowMyStatus(playerId, myInfo);
            return Unit.Value;
        }

        public async Task<Unit> Handle(PingCommand command, CancellationToken cancellationToken)
        {
            var playerId = command.PlayerId;

            await _queueHandler.SendQueueMessage(new ReceiveEmailQueue(playerId));

            //更新玩家在线数据
            var model = await _mudOnlineProvider.GetPlayerOnline(playerId);
            if (model != null)
            {
                await _mudOnlineProvider.SetPlayerOnline(new PlayerOnlineModel
                {
                    IsOnline = true,
                    LastDate = DateTime.Now,
                    Level = model.Level,
                    PlayerName = model.PlayerName,
                    PlayerId = model.PlayerId,
                    RoomId = model.RoomId,
                    Gender = model.Gender,
                    Title = model.Title
                });
                return Unit.Value;
            }

            var player = await _playerDomainService.Get(playerId);
            await _mudOnlineProvider.SetPlayerOnline(new PlayerOnlineModel
            {
                IsOnline = true,
                LastDate = DateTime.Now,
                Level = player.Level,
                PlayerName = player.Name,
                PlayerId = player.Id,
                RoomId = player.RoomId,
                Gender = player.Gender,
                Title = player.Title
            });




            return Unit.Value;
        }

        public async Task<Unit> Handle(SendMessageCommand command, CancellationToken cancellationToken)
        {
            var playerId = command.PlayerId;
            var channel = command.Channel;
            var content = command.Content;

            var receivedMessage = new PlayerMessage()
            {
                Channel = "闲聊",
                Content = WebUtility.HtmlEncode(content),
                Sender = _account.PlayerName,
                PlayerId = _account.PlayerId
            };

            await _mudProvider.ShowChat(receivedMessage);


            await _bus.RaiseEvent(new SendMessageEvent(playerId, content)).ConfigureAwait(false);

            //新手任务
            var chatTimes = await _redisDb.StringGet<int>(string.Format(RedisKey.ChatTimes, playerId));
            if (chatTimes <= 0)
            {
                await _queueHandler.SendQueueMessage(new CompleteQuestNewbieQuestQueue(playerId, NewbieQuestEnum.第一次聊天));
            }
            chatTimes++;
            await _redisDb.StringSet(string.Format(RedisKey.ChatTimes, playerId), chatTimes);

            return Unit.Value;
        }

        public async Task<Unit> Handle(ShowPlayerCommand command, CancellationToken cancellationToken)
        {
            var myId = command.MyId;
            var me = await _playerDomainService.Get(myId);
            if (me == null)
            {
                return Unit.Value;
            }


            var playerId = command.PlayerId;
            var playerInfo = new PlayerInfo()
            {
                Descriptions = new List<string>(),
                Commands = new List<PlayerCommandModel>()
            };
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                return Unit.Value;
            }
            playerInfo.Id = playerId;
            playerInfo.Name = player.Name;
            string genderStr = player.Gender.ToGender();


            //年龄
            playerInfo.Descriptions.Add($"{genderStr}{player.Age.ToAge()}");


            playerInfo.Descriptions.Add($"{genderStr}的武功看不出深浅。");
            playerInfo.Descriptions.Add($"{genderStr}看起来气血充盈，并没有受伤。");

            if (me.RoomId == player.RoomId)
            {
                playerInfo.Commands.Add(new PlayerCommandModel("切磋"));
                playerInfo.Commands.Add(new PlayerCommandModel("杀死"));
            }

            var playerRelationFrom = await _playerRelationDomainService.Get(x => x.Type == PlayerRelationTypeEnum.好友 && x.PlayerId == myId && x.RelationId == playerId);

            var playerRelationTo = await _playerRelationDomainService.Get(x => x.Type == PlayerRelationTypeEnum.好友 && x.PlayerId == playerId && x.RelationId == myId);

            if (playerRelationFrom == null)
            {
                playerInfo.Commands.Add(new PlayerCommandModel("添加好友"));
            }
            if (playerRelationFrom != null && playerRelationTo != null)
            {
                playerInfo.Commands.Add(new PlayerCommandModel("割袍断义", $"是否要与[{player.Name}]取消好友关系？"));
                
                playerInfo.Commands.Add(new PlayerCommandModel("查看武功"));
            }


            await _mudProvider.ShowPlayer(myId, playerInfo);

            return Unit.Value;
        }

    }
}
