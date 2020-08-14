using AutoMapper;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.CommandHandlers;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Notifications;
using Emprise.Domain.Player.Models;
using Emprise.Domain.Player.Services;
using Emprise.Domain.PlayerRelation.Services;
using Emprise.MudServer.Commands.RelationCommonds;
using Emprise.MudServer.Models;
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
        IRequestHandler<ShowFriendCommand, Unit>

    {
        private readonly IMediatorHandler _bus;
        private readonly ILogger<RelationCommandHandler> _logger;
        private readonly IPlayerRelationDomainService _playerRelationDomainService;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly IRedisDb _redisDb;
        private readonly IMudProvider _mudProvider;


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
    }
}
