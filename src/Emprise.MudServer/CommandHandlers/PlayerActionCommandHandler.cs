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
using Emprise.Domain.Player.Entity;
using Emprise.Domain.Player.Services;
using Emprise.Domain.PlayerRelation.Entity;
using Emprise.Domain.PlayerRelation.Services;
using Emprise.MudServer.Commands;
using Emprise.MudServer.Commands.RelationCommonds;
using Emprise.MudServer.Queues;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emprise.MudServer.CommandHandlers
{

    public class PlayerActionCommandHandler : CommandHandler,
        IRequestHandler<PlayerActionCommand, Unit>

    {
        private readonly IMediatorHandler _bus;
        private readonly ILogger<PlayerActionCommandHandler> _logger;
        private readonly IPlayerRelationDomainService  _playerRelationDomainService;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly IEmailDomainService _emailDomainService;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly IRedisDb _redisDb;
        private readonly IMudProvider _mudProvider;
        private readonly IQueueHandler _queueHandler;

        public PlayerActionCommandHandler(
            IMediatorHandler bus,
            ILogger<PlayerActionCommandHandler> logger,
            IPlayerRelationDomainService playerRelationDomainService,
            IPlayerDomainService playerDomainService,
            IEmailDomainService emailDomainService,
            IMapper mapper,
            IMemoryCache cache,
            IRedisDb redisDb,
            IMudProvider mudProvider,
            INotificationHandler<DomainNotification> notifications,
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

        
        private async Task Friend(PlayerEntity player, PlayerEntity relation)
        {

            if (await _redisDb.StringGet<int>(string.Format(RedisKey.RefuseFriend, player.Id, relation.Id)) > 0)
            {
                await _mudProvider.ShowMessage(player.Id, $"【好友】[{relation.Name}]已拒绝你的申请。");
                return;
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
                    return;
                }
                else
                {
                    await _mudProvider.ShowMessage(player.Id, $"【好友】你已申请加[{relation.Name}]为好友，请等待对方同意。");
                    return;
                }

            }



            if (playerRelationFrom == null)
            {
                if(playerRelationTo == null)
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

        }
        
        private async Task UnFriend(PlayerEntity player, PlayerEntity relation)
        {

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
        }

        private async Task ShowFriendSkill(PlayerEntity player, PlayerEntity relation)
        {

        }


        public async Task<Unit> Handle(PlayerActionCommand command, CancellationToken cancellationToken)
        {

            var targetId = command.TargetId;
            var playerId = command.PlayerId;
            var commandName = command.CommandName;

            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            var target = await _playerDomainService.Get(targetId);
            if (target == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"npc不存在！"));
                return Unit.Value;
            }


            //await DoPlayerAction(player, target, commandName);

            PlayerActionEnum actionEnum;
            if (!Enum.TryParse(commandName, out actionEnum))
            {
                return Unit.Value;
            }

            switch (actionEnum)
            {
                case PlayerActionEnum.添加好友:
                    await Friend(player, target);
                    break;

                case PlayerActionEnum.割袍断义:
                    await UnFriend(player, target);
                    break;

                case PlayerActionEnum.查看武功:
                    await ShowFriendSkill(player, target);
                    break;
            }

            if (await Commit())
            {
                //Do nothing
            }

            return Unit.Value;
        }



    }
}
