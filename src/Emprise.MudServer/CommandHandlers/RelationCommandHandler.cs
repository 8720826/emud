using AutoMapper;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.CommandHandlers;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Notifications;
using Emprise.Domain.Email.Entity;
using Emprise.Domain.Email.Services;
using Emprise.Domain.Player.Models;
using Emprise.Domain.Player.Services;
using Emprise.Domain.PlayerRelation.Entity;
using Emprise.Domain.PlayerRelation.Services;
using Emprise.MudServer.Commands.RelationCommonds;
using Emprise.MudServer.Models;
using Emprise.MudServer.Queues;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emprise.MudServer.CommandHandlers
{

    public class RelationCommandHandler : CommandHandler,
        IRequestHandler<ShowFriendCommand, Unit>,
        IRequestHandler<AgreeFriendCommand, Unit>,
        IRequestHandler<RefuseFriendCommand, Unit>,
        IRequestHandler<FriendToCommand, Unit>,
        IRequestHandler<UnFriendToCommand, Unit>

        

    {
        private readonly IMediatorHandler _bus;
        private readonly ILogger<RelationCommandHandler> _logger;
        private readonly IPlayerRelationDomainService _playerRelationDomainService;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly IEmailDomainService _emailDomainService;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly IRedisDb _redisDb;
        private readonly IMudProvider _mudProvider;
        private readonly IQueueHandler _queueHandler;

        public RelationCommandHandler(
            IMediatorHandler bus,
            ILogger<RelationCommandHandler> logger,
            IPlayerRelationDomainService playerRelationDomainService,
            IPlayerDomainService playerDomainService,
            IMapper mapper,
            IMemoryCache cache,
            IRedisDb redisDb,
            IMudProvider mudProvider,
            INotificationHandler<DomainNotification> notifications,
            IEmailDomainService emailDomainService,
            IQueueHandler queueHandler,
            IUnitOfWork uow) : base(uow, bus, notifications)
        {

            _bus = bus;
            _logger = logger;
            _playerRelationDomainService = playerRelationDomainService;
            _mapper = mapper;
            _cache = cache;
            _playerDomainService = playerDomainService;
            _emailDomainService = emailDomainService;
            _redisDb = redisDb;
            _mudProvider = mudProvider;
            _queueHandler = queueHandler;
        }


        public async Task<Unit> Handle(ShowFriendCommand command, CancellationToken cancellationToken)
        {

            var playerId = command.PlayerId;

            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            var playerRelationQuery = await _playerRelationDomainService.GetAll();
            //我加了别人的
            var requestIds = playerRelationQuery.Where(x => x.PlayerId == playerId && x.Type == PlayerRelationTypeEnum.好友).Select(x=>x.RelationId).ToList();

            //别人加了我的
            var friendToMeIds = playerRelationQuery.Where(x => x.RelationId == playerId && x.Type == PlayerRelationTypeEnum.好友).Select(x => x.PlayerId).ToList();

            //互相加了的
            var friendIds = requestIds.Intersect(friendToMeIds).ToList();

            requestIds = requestIds.Except(friendIds).ToList();
            friendToMeIds = friendToMeIds.Except(friendIds).ToList();

            var playerQuery = await _playerDomainService.GetAll();

            var players = playerQuery.Where(x=> friendIds.Contains(x.Id)).ToList();
            var requests = playerQuery.Where(x => requestIds.Contains(x.Id)).ToList();
            var friendToMes = playerQuery.Where(x => friendToMeIds.Contains(x.Id)).ToList();

            await _mudProvider.ShowFriend(playerId, new MyFriendModel
            {
                Friends = _mapper.Map<List<PlayerBaseInfo>>(players),
                Requests = _mapper.Map<List<PlayerBaseInfo>>(requests),
                FriendMes = _mapper.Map<List<PlayerBaseInfo>>(friendToMes)
            });



            return Unit.Value;
        }

        public async Task<Unit> Handle(AgreeFriendCommand command, CancellationToken cancellationToken)
        {
            var playerId = command.PlayerId;
            var relationId = command.RelationId;
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            var relation = await _playerDomainService.Get(relationId);
            if (relation == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            var playerRelationFrom = await _playerRelationDomainService.Get(x => x.PlayerId == relationId && x.RelationId == playerId && x.Type == PlayerRelationTypeEnum.好友);
            if (playerRelationFrom == null)
            {
                await _mudProvider.ShowMessage(player.Id, $"【好友】[{relation.Name}]没有申请加你为好友，或者已撤销申请。");
                return Unit.Value;
            }

            var playerRelationTo = await _playerRelationDomainService.Get(x => x.PlayerId == playerId && x.RelationId == relationId && x.Type == PlayerRelationTypeEnum.好友);
            if (playerRelationTo != null)
            {
                await _mudProvider.ShowMessage(player.Id, $"【好友】你和[{relation.Name}]已经是好友了。");
                return Unit.Value;
            }


            playerRelationTo = new PlayerRelationEntity
            {
                PlayerId = playerId,
                RelationId = relationId,
                Type = PlayerRelationTypeEnum.好友,
                CreatedTime = DateTime.Now
            };
            await _playerRelationDomainService.Add(playerRelationTo);

            await _mudProvider.ShowMessage(player.Id, $"【好友】你同意了[{relation.Name}]的好友申请。");

            var content = $"【好友】[{player.Name}]同意了你的申请，你们已成为了好友。";

            await _emailDomainService.Add(new EmailEntity
            {
                ExpiryDate = DateTime.Now.AddDays(30),
                SendDate = DateTime.Now,
                Title = $"{player.Name}同意了你的好友申请",
                Content = content,
                Type = EmailTypeEnum.系统,
                TypeId = relation.Id
            });


            await _mudProvider.ShowMessage(relation.Id, content);

            return Unit.Value;
        }

        public async Task<Unit> Handle(RefuseFriendCommand command, CancellationToken cancellationToken)
        {
            var playerId = command.PlayerId;
            var relationId = command.RelationId;
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            var relation = await _playerDomainService.Get(relationId);
            if (relation == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            var playerRelationFrom = await _playerRelationDomainService.Get(x => x.PlayerId == relationId && x.RelationId == playerId && x.Type == PlayerRelationTypeEnum.好友);
            if (playerRelationFrom == null)
            {
                await _mudProvider.ShowMessage(player.Id, $"【好友】[{relation.Name}]没有申请加你为好友，或者已撤销申请。");
                return Unit.Value;
            }

            var playerRelationTo = await _playerRelationDomainService.Get(x => x.PlayerId == playerId && x.RelationId == relationId && x.Type == PlayerRelationTypeEnum.好友);
            if (playerRelationTo != null)
            {
                await _mudProvider.ShowMessage(player.Id, $"【好友】你和[{relation.Name}]已经是好友了。");
                return Unit.Value;
            }


            await _playerRelationDomainService.Delete(playerRelationFrom);

            //1天内不得重复申请
            await _redisDb.StringSet(string.Format(RedisKey.RefuseFriend, relationId, playerId), 1, DateTime.Now.AddDays(1));

            await _mudProvider.ShowMessage(player.Id, $"【好友】你拒绝了[{relation.Name}]的好友申请。");

            var content = $"【好友】[{player.Name}]拒绝了你的好友申请。";

            await _emailDomainService.Add(new EmailEntity
            {
                ExpiryDate = DateTime.Now.AddDays(30),
                SendDate = DateTime.Now,
                Title = $"[{player.Name}]拒绝了你的好友申请",
                Content = content,
                Type = EmailTypeEnum.系统,
                TypeId = relation.Id
            });


            await _mudProvider.ShowMessage(relation.Id, content);

            return Unit.Value;
        }

        public async Task<Unit> Handle(FriendToCommand command, CancellationToken cancellationToken)
        {
            var playerId = command.PlayerId;
            var relationId = command.RelationId;
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            var relation = await _playerDomainService.Get(relationId);
            if (relation == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            if (await _redisDb.StringGet<int>(string.Format(RedisKey.RefuseFriend, player.Id, relation.Id)) > 0)
            {
                await _mudProvider.ShowMessage(player.Id, $"【好友】[{relation.Name}]已拒绝你的申请。");
                return Unit.Value;
            }

            //我加对方
            var playerRelationFrom = await _playerRelationDomainService.Get(x => x.PlayerId == player.Id
                && x.RelationId == relation.Id
                && x.Type == PlayerRelationTypeEnum.好友);

            //对方加我
            var playerRelationTo = await _playerRelationDomainService.Get(x => x.PlayerId == relation.Id
                && x.RelationId == player.Id
                && x.Type == PlayerRelationTypeEnum.好友);



            if (playerRelationFrom != null && playerRelationTo != null)
            {
                if (playerRelationTo != null)
                {
                    await _mudProvider.ShowMessage(player.Id, $"【好友】你们已经是好友。");
                    return Unit.Value;
                }
                else
                {
                    await _mudProvider.ShowMessage(player.Id, $"【好友】你已申请加[{relation.Name}]为好友，请等待对方同意。");
                    return Unit.Value;
                }

            }



            if (playerRelationFrom == null)
            {
                if (playerRelationTo == null)
                {

                    await _mudProvider.ShowMessage(player.Id, $"【好友】你申请加[{relation.Name}]为好友，请等待对方同意。");

                    var content = $"【好友】[{player.Name}]想和你成为好友，到 '社交'->'好友' 界面可以同意或拒绝对方的申请，你也可以直接添加对方为好友。";

                    await _emailDomainService.Add(new EmailEntity
                    {
                        ExpiryDate = DateTime.Now.AddDays(30),
                        SendDate = DateTime.Now,
                        Title = $"{player.Name}想和你成为好友",
                        Content = content,
                        Type = EmailTypeEnum.系统,
                        TypeId = relation.Id
                    });


                    await _mudProvider.ShowMessage(relation.Id, content);
                }
                else
                {
                    await _mudProvider.ShowMessage(player.Id, $"【好友】你成功添加[{relation.Name}]为好友。");


                    var content = $"【好友】[{player.Name}]同意了你的申请，你们已成为了好友。";

                    await _emailDomainService.Add(new EmailEntity
                    {
                        ExpiryDate = DateTime.Now.AddDays(30),
                        SendDate = DateTime.Now,
                        Title = $"{player.Name}同意了你的好友申请",
                        Content = content,
                        Type = EmailTypeEnum.系统,
                        TypeId = relation.Id
                    });


                    await _mudProvider.ShowMessage(relation.Id, content);
                }


                playerRelationFrom = new PlayerRelationEntity
                {
                    CreatedTime = DateTime.Now,
                    PlayerId = player.Id,
                    RelationId = relation.Id,
                    Type = PlayerRelationTypeEnum.好友
                };
                await _playerRelationDomainService.Add(playerRelationFrom);



                await _queueHandler.SendQueueMessage(new ReceiveEmailQueue(relation.Id));
            }
            return Unit.Value;
        }

        public async Task<Unit> Handle(UnFriendToCommand command, CancellationToken cancellationToken)
        {
            var playerId = command.PlayerId;
            var relationId = command.RelationId;
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            var relation = await _playerDomainService.Get(relationId);
            if (relation == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            //我加对方
            var playerRelationFrom = await _playerRelationDomainService.Get(x => x.PlayerId == player.Id
                && x.RelationId == relation.Id
                && x.Type == PlayerRelationTypeEnum.好友);

            //对方加我
            var playerRelationTo = await _playerRelationDomainService.Get(x => x.PlayerId == relation.Id
                && x.RelationId == player.Id
                && x.Type == PlayerRelationTypeEnum.好友);



            if (playerRelationFrom != null && playerRelationTo != null)
            {
                await _playerRelationDomainService.Delete(playerRelationFrom);

                await _playerRelationDomainService.Delete(playerRelationTo);

                //1天内不得重复申请
                await _redisDb.StringSet(string.Format(RedisKey.RefuseFriend, relation.Id, player.Id), 1, DateTime.Now.AddDays(1));

                await _mudProvider.ShowMessage(player.Id, $"【好友】你已经与[{relation.Name}]取消好友关系。");

                var content = $"【好友】[{player.Name}]已经与你取消好友关系。";

                await _emailDomainService.Add(new EmailEntity
                {
                    ExpiryDate = DateTime.Now.AddDays(30),
                    SendDate = DateTime.Now,
                    Title = $"{player.Name}与你取消好友关系",
                    Content = content,
                    Type = EmailTypeEnum.系统,
                    TypeId = relation.Id
                });


                await _mudProvider.ShowMessage(relation.Id, content);
            }
            else
            {
                await _mudProvider.ShowMessage(player.Id, $"【好友】你们并不是好友。");
            }

            return Unit.Value;
        }
    }
}
