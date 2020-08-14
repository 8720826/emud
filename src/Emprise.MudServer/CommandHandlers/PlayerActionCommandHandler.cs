using AutoMapper;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.CommandHandlers;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Notifications;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.Player.Services;
using Emprise.Domain.PlayerRelation.Entity;
using Emprise.Domain.PlayerRelation.Services;
using Emprise.MudServer.Commands;
using Emprise.MudServer.Commands.RelationCommonds;
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
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly IRedisDb _redisDb;
        private readonly IMudProvider _mudProvider;


        public PlayerActionCommandHandler(
            IMediatorHandler bus,
            ILogger<PlayerActionCommandHandler> logger,
            IPlayerRelationDomainService playerRelationDomainService,
            IPlayerDomainService playerDomainService,
            IMapper mapper,
            IMemoryCache cache,
            IRedisDb redisDb,
            IMudProvider mudProvider,
            INotificationHandler<DomainNotification> notifications,
            IUnitOfWork uow) : base(uow, bus, notifications)
        {

            _bus = bus;
            _logger = logger;
            _playerRelationDomainService = playerRelationDomainService;
            _mapper = mapper;
            _cache = cache;
            _playerDomainService = playerDomainService;
            _redisDb = redisDb;
            _mudProvider = mudProvider;
        }

        
        public async Task Friend(PlayerEntity player, PlayerEntity relation)
        {
            var playerRelation = await _playerRelationDomainService.Get(x => x.PlayerId == player.Id
            && x.RelationId == relation.Id
            && x.Type == PlayerRelationTypeEnum.好友);
            if (playerRelation != null)
            {
                await _mudProvider.ShowMessage(player.Id, $"你已申请加{relation.Name}为好友，请等待对方同意。");
                return;
            }

            playerRelation = new PlayerRelationEntity
            {
                CreatedTime = DateTime.Now,
                PlayerId = player.Id,
                RelationId = relation.Id,
                Type = PlayerRelationTypeEnum.好友
            };
            await _playerRelationDomainService.Add(playerRelation);
            await _mudProvider.ShowMessage(player.Id, $"你已申请加{relation.Name}为好友，请等待对方同意。");
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
            }

            if (await Commit())
            {
                //Do nothing
            }

            return Unit.Value;
        }



    }
}
