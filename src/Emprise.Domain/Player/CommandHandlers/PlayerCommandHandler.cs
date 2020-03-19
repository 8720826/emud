using AutoMapper;
using Emprise.Domain.Core.Authorization;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Bus.Models;
using Emprise.Domain.Core.CommandHandlers;
using Emprise.Domain.Core.Enum;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Notifications;
using Emprise.Domain.Npc.Services;
using Emprise.Domain.Player.Commands;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.Player.Events;
using Emprise.Domain.Player.Services;
using Emprise.Domain.Room.Services;
using Emprise.Infra.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emprise.Domain.User.CommandHandlers
{
    
    public class PlayerCommandHandler : CommandHandler, 
        IRequestHandler<CreateCommand, Unit>,
        IRequestHandler<JoinGameCommand, Unit>,
        IRequestHandler<InitGameCommand, Unit>,
        IRequestHandler<MoveCommand, Unit>,
        IRequestHandler<SearchCommand, Unit>,
        IRequestHandler<MeditateCommand, Unit>,
        IRequestHandler<StopActionCommand, Unit>,
        IRequestHandler<ExertCommand, Unit>,
        IRequestHandler<NpcActionCommand, Unit>


        
    {
        private readonly IMediatorHandler _bus;
        private readonly ILogger<PlayerCommandHandler> _logger;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly IMapper _mapper;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly IRoomDomainService _roomDomainService;
        private readonly INpcDomainService _npcDomainService;
        private readonly IAccountContext _account;
        private readonly IDelayedQueue  _delayedQueue;
        private readonly IRecurringQueue _recurringQueue;
        private readonly IMudProvider _mudProvider;
        private readonly AppConfig _appConfig;
        private readonly INpcScriptDomainService _npcScriptDomainService;
        private readonly INpcScriptCommandDomainService _npcScriptCommandDomainService;
        private readonly IRedisDb _redisDb;

        public PlayerCommandHandler(
            IMediatorHandler bus,
            ILogger<PlayerCommandHandler> logger,
            IHttpContextAccessor httpAccessor,
            IMapper mapper,
            IPlayerDomainService playerDomainService,
            IRoomDomainService roomDomainService,
            INpcDomainService npcDomainService,
            IAccountContext account,
            IDelayedQueue delayedQueue,
            IRecurringQueue recurringQueue,
            IMudProvider mudProvider,
            IOptions<AppConfig> appConfig,
            INpcScriptDomainService npcScriptDomainService,
            INpcScriptCommandDomainService npcScriptCommandDomainService,
            IRedisDb redisDb,
            INotificationHandler<DomainNotification> notifications) : base(bus, notifications)
        {
           

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
            _appConfig = appConfig.Value;
            _npcScriptDomainService = npcScriptDomainService;
            _npcScriptCommandDomainService = npcScriptCommandDomainService;
            _npcDomainService = npcDomainService;
            _redisDb = redisDb;
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

            player = await _playerDomainService.GetUserPlayer(userId);
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
                Atk = str,
                Str = str,
                Con = con,
                Int = @int,
                Dex = dex,
                Exp = 0,
                Cor = 20,
                Cps = 20,
                Flee = 0,
                Hit = 0,
                Pot = 0,
                Kar = 20,
                Def = 0,
                Hp = 0,
                LimitMp = 0,
                MaxHp = 0,
                MaxMp = 0,
                Mp = 0,
                Parry = 0,
                Per = 0,
                Nrg = 0
            };


            await _playerDomainService.Add(player);

            var jwtAccount = new JwtAccount
            {
                UserId = userId,
                Email = _account.Email,
                PlayerId = player.Id,
                PlayerName = player.Name
            };

            await _httpAccessor.HttpContext.SignIn(CookieAuthenticationDefaults.AuthenticationScheme, jwtAccount);

            await _bus.RaiseEvent(new CreatedEvent(player)).ConfigureAwait(false);

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

            await _httpAccessor.HttpContext.SignIn(CookieAuthenticationDefaults.AuthenticationScheme, jwtAccount);

            await _bus.RaiseEvent(new JoinedGameEvent(player)).ConfigureAwait(false);

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
                await _bus.RaiseEvent(new DomainNotification("场景不存在！"));
                return Unit.Value;
            }
            await _bus.RaiseEvent(new InitGameEvent(player)).ConfigureAwait(false);
            await _bus.RaiseEvent(new PlayerInRoomEvent(player, room)).ConfigureAwait(false);
        

            return Unit.Value;
        }


        public async Task<Unit> Handle(MoveCommand command, CancellationToken cancellationToken)
        {

            var roomId = command.RoomId;
            var playerId = command.PlayerId;
           
            if (roomId <= 0)
            {
                await _bus.RaiseEvent(new DomainNotification($"场景不存在！"));
                return Unit.Value;
            }

            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }
            if (player.Status!= PlayerStatusEnum.空闲)
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
                    await _bus.RaiseEvent(new DomainNotification($"无法移动到该场景！"));
                    return Unit.Value;
                }
            }

            var newRoom = await _roomDomainService.Get(roomId);
            if (newRoom == null)
            {
                await _bus.RaiseEvent(new DomainNotification("场景不存在！"));
                return Unit.Value;
            }

            player.RoomId = roomId;
            await _playerDomainService.Update(player);

            /*
            var jwtAccount = new JwtAccount
            {
                UserId = _account.UserId,
                Email = _account.Email,
                PlayerId = _account.PlayerId,
                PlayerName = _account.PlayerName
            };

            await _httpAccessor.HttpContext.SignIn(CookieAuthenticationDefaults.AuthenticationScheme, jwtAccount);
            */

            //await _bus.RaiseEvent(new MovedEvent(player)).ConfigureAwait(false);

            await _bus.RaiseEvent(new MovedEvent(player, newRoom, oldRoom));
            

            return Unit.Value;
        }




        public async Task<Unit> Handle(SearchCommand command, CancellationToken cancellationToken)
        {
            var playerId = command.PlayerId;
            //await _bus.RaiseEvent(new DomainNotification("你什么也没发现"));
            await _mudProvider.ShowMessage(playerId, "你什么也没发现。。。");
            return Unit.Value;
        }

        public async Task<Unit> Handle(MeditateCommand command, CancellationToken cancellationToken)
        {
            //await _bus.RaiseEvent(new DomainNotification("功能暂时未实现"));

            var playerId = command.PlayerId;
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            if (player.Status == PlayerStatusEnum.打坐)
            {
                return Unit.Value;
            }

            if (player.Status != PlayerStatusEnum.空闲)
            {
                await _bus.RaiseEvent(new DomainNotification($"你正在{player.Status}中，请先停下！"));
                return Unit.Value;
            }

            await _mudProvider.ShowMessage(playerId, "开始打坐。。。");
            player.Status = PlayerStatusEnum.打坐;
            await _playerDomainService.Update(player);

            await _recurringQueue.Publish(playerId, new MeditateModel { }, 2,10);

            await _bus.RaiseEvent(new PlayerStatusChangedEvent(player)).ConfigureAwait(false);

            return Unit.Value;
        }

        public async Task<Unit> Handle(StopActionCommand command, CancellationToken cancellationToken)
        {
            //await _bus.RaiseEvent(new DomainNotification("功能暂时未实现"));

            var playerId = command.PlayerId;
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }
            var status = player.Status;
            if (status == PlayerStatusEnum.空闲)
            {
                return Unit.Value;
            }


            player.Status = PlayerStatusEnum.空闲;
            await _playerDomainService.Update(player);

            switch (status)
            {
                case PlayerStatusEnum.打坐:
                    await _mudProvider.ShowMessage(playerId, "你停止了打坐。。。");
                    await _recurringQueue.Remove<MeditateModel>(playerId);
                    break;

                case PlayerStatusEnum.工作:
                    await _mudProvider.ShowMessage(playerId, "你停止了工作。。。");
                    await _recurringQueue.Remove<MeditateModel>(playerId);
                    break;
            }
          

            await _bus.RaiseEvent(new PlayerStatusChangedEvent(player)).ConfigureAwait(false);

            return Unit.Value;
        }


        
        public async Task<Unit> Handle(ExertCommand command, CancellationToken cancellationToken)
        {
            //await _bus.RaiseEvent(new DomainNotification("功能暂时未实现"));

            var playerId = command.PlayerId;
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }
            var status = player.Status;
            if (status == PlayerStatusEnum.疗伤)
            {
                return Unit.Value;
            }

            if (player.Status != PlayerStatusEnum.空闲)
            {
                await _bus.RaiseEvent(new DomainNotification($"你正在{player.Status}中，请先停下！"));
                return Unit.Value;
            }

            await _mudProvider.ShowMessage(playerId, "你盘膝坐下，开始运功疗伤。");
            player.Status = PlayerStatusEnum.疗伤;
            await _playerDomainService.Update(player);

            await _recurringQueue.Publish(playerId, new ExertModel {  }, 2, 10);

            await _bus.RaiseEvent(new PlayerStatusChangedEvent(player)).ConfigureAwait(false);


            return Unit.Value;
        }

        public async Task<Unit> Handle(NpcActionCommand command, CancellationToken cancellationToken)
        {

            var npcId = command.NpcId;
            var playerId = command.PlayerId;
            var action = command.Action;
            var scriptId = command.ScriptId;
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            var npc = await _npcDomainService.Get(npcId);
            if (npc == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"npc不存在！"));
                return Unit.Value;
            }

            if (scriptId > 0)
            {

                if (string.IsNullOrEmpty(npc.Scripts))
                {
                    await _bus.RaiseEvent(new DomainNotification($"脚本不存在！"));
                    return Unit.Value;
                }

                var dic = JsonConvert.DeserializeObject<Dictionary<int, string>>(npc.Scripts);
                if (dic == null || dic.Count == 0)
                {
                    await _bus.RaiseEvent(new DomainNotification($"脚本不存在！"));
                    return Unit.Value;
                }

                var scriptIds = dic.Select(x => x.Key).ToList();
                if (!scriptIds.Contains(scriptId))
                {
                    await _bus.RaiseEvent(new DomainNotification($"脚本不存在！"));
                    return Unit.Value;
                }

                var npcScript = await _npcScriptDomainService.Get(scriptId);
                if (npcScript == null)
                {
                    await _bus.RaiseEvent(new DomainNotification($"脚本不存在！"));
                    return Unit.Value;
                }

                var scriptCommands = await _npcScriptCommandDomainService.Query(x => x.ScriptId == scriptId);

                var scriptCommand = scriptCommands.FirstOrDefault(x => string.Equals(x.ActionName, action, StringComparison.InvariantCultureIgnoreCase));
                if (scriptCommand == null)
                {
                    return Unit.Value;
                }

                if (!scriptCommand.IsEntry)
                {
                    var commandIds = await _redisDb.StringGet<List<int>>($"commandIds_{npc.Id}_{scriptId}");
                    if (!commandIds.Contains(scriptCommand.Id))
                    {
                        return Unit.Value;
                    }
                }

                var caseIf = scriptCommand.CaseIf;

                await _bus.RaiseEvent(new DomainNotification($"执行指令 {action}！"));
                return Unit.Value;
            }
            else
            {
                NpcActionEnum actionEnum;
                if (Enum.TryParse(action, out actionEnum))
                {
                    switch (actionEnum)
                    {
                        case NpcActionEnum.切磋:
                            if (!npc.CanFight)
                            {
                                await _bus.RaiseEvent(new DomainNotification($"指令 {action} 错误！"));
                                return Unit.Value;
                            }
                            else
                            {
                                await _bus.RaiseEvent(new DomainNotification($"指令 {action} 未实现！"));
                                return Unit.Value;
                            }
                            break;

                        case NpcActionEnum.杀死:
                            if (!npc.CanKill)
                            {
                                await _bus.RaiseEvent(new DomainNotification($"指令 {action} 错误！"));
                                return Unit.Value;
                            }
                            else
                            {
                                await _bus.RaiseEvent(new DomainNotification($"指令 {action} 未实现！"));
                                return Unit.Value;
                            }
                            break;

                        case NpcActionEnum.给予:
                            if (npc.Type != NpcTypeEnum.人物)
                            {
                                await _bus.RaiseEvent(new DomainNotification($"指令 {action} 错误！"));
                                return Unit.Value;
                            }
                            else
                            {
                                await _bus.RaiseEvent(new DomainNotification($"指令 {action} 未实现！"));
                                return Unit.Value;
                            }
                            break;
                    }
                }
            }



            return Unit.Value;

            /*
            var type = Type.GetType("Emprise.MudServer.Scripts." + npc.Script + ",Emprise.MudServer", false, true);
            if (type != null)
            {
                using (var serviceScope = _services.CreateScope())
                {
                    var argtypes = type.GetConstructors()
                    .First()
                    .GetParameters()
                    .Select(x =>
                    {
                        if (x.Name == "player")
                            return player;
                        else if (x.Name == "npc")
                            return npc;
                        else if (x.ParameterType == typeof(IServiceProvider))
                            return serviceScope.ServiceProvider;
                        else
                            return serviceScope.ServiceProvider.GetService(x.ParameterType);
                    })
                    .ToArray();

                    +var job = Activator.CreateInstance(type, argtypes);
                    if (!hasCheckAction)
                    {
                        MethodInfo method = type.GetMethod("GetActions");
                        var actions = (List<string>)method.Invoke(job, new object[] { });

                        if (!actions.Contains(action))
                        {
                            await _bus.RaiseEvent(new DomainNotification($"指令 {action} 错误！"));
                            return Unit.Value;
                        }
                    }

 
                }

            }
            */

        }
        
    }
}
