using AutoMapper;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.CommandHandlers;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Notifications;
using Emprise.MudServer.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emprise.Domain.Npc.Services;
using Emprise.Domain.Player.Services;
using Emprise.Domain.Ware.Services;
using Microsoft.Extensions.Caching.Memory;
using Emprise.Domain.Ware.Models;
using Emprise.Domain.Player.Models;

namespace Emprise.MudServer.CommandHandlers
{
    
    public class WareCommandHandler : CommandHandler,
        IRequestHandler<ShowMyPackCommand, Unit>,
        IRequestHandler<LoadWareCommand, Unit>,
        IRequestHandler<UnLoadWareCommand, Unit>

    {
        private readonly IMediatorHandler _bus;
        private readonly ILogger<WareCommandHandler> _logger;
        private readonly INpcDomainService _npcDomainService;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly IWareDomainService _wareDomainService;
        private readonly IPlayerWareDomainService _playerWareDomainService;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly IRedisDb _redisDb;
        private readonly IMudProvider _mudProvider;


        public WareCommandHandler(
            IMediatorHandler bus,
            ILogger<WareCommandHandler> logger,
            INpcDomainService npcDomainService,
            IPlayerDomainService playerDomainService,
            IWareDomainService wareDomainService,
            IPlayerWareDomainService playerWareDomainService,
            IMapper mapper,
            IMemoryCache cache, 
            IRedisDb redisDb,
            IMudProvider mudProvider,
            INotificationHandler<DomainNotification> notifications,
            IUnitOfWork uow) : base(uow, bus, notifications)
        {

            _bus = bus;
            _logger = logger;
            _npcDomainService = npcDomainService;
            _mapper = mapper;
            _cache = cache;
            _playerDomainService = playerDomainService;
            _wareDomainService = wareDomainService;
            _playerWareDomainService = playerWareDomainService;
            _redisDb = redisDb;
            _mudProvider = mudProvider;
        }

        public async Task<Unit> Handle(ShowMyPackCommand command, CancellationToken cancellationToken)
        {
            var playerId = command.PlayerId;
            var myPack = new MyPack()
            {
                Money = "",
                Wares = new List<WareModel>()

            };
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                return Unit.Value;
            }

            myPack.Money = player.Money.ToMoney();

            var playerWares = await _playerWareDomainService.GetAll(player.Id);
            if (playerWares == null)
            {
                return Unit.Value;
            }

            var ids = playerWares.Select(x => x.WareId);

            var wares = (await _wareDomainService.GetAll()).Where(x => ids.Contains(x.Id));
            foreach (var playerWare in playerWares)
            {
                var ware = wares.FirstOrDefault(x => x.Id == playerWare.WareId);
                if (ware != null)
                {
                    var wareModel = _mapper.Map<WareModel>(ware);
                    wareModel.Number = playerWare.Number;
                    wareModel.Status = playerWare.Status;
                    myPack.Wares.Add(wareModel);
                }

            }

            await _mudProvider.ShowMyPack(playerId, myPack);

            return Unit.Value;
        }


        public async Task<Unit> Handle(LoadWareCommand command, CancellationToken cancellationToken)
        {
            var wareId = command.WareId;
            var playerId = command.PlayerId;

            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            var ware = await _wareDomainService.Get(wareId);
            if (ware == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"武器不存在！"));
                return Unit.Value;
            }


            return Unit.Value;
        }

        public async Task<Unit> Handle(UnLoadWareCommand command, CancellationToken cancellationToken)
        {
            var wareId = command.WareId;
            var playerId = command.PlayerId;

            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            var ware = await _wareDomainService.Get(wareId);
            if (ware == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"武器不存在！"));
                return Unit.Value;
            }

            return Unit.Value;
        }
    }
}
