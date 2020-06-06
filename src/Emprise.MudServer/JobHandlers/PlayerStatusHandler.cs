using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Interfaces.Ioc;
using Emprise.Domain.Core.Queue.Models;
using Emprise.Domain.Player.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.MudServer.Handles
{
    public interface IPlayerStatusHandler : IScoped
    {
        Task Execute(PlayerStatusModel model);
    }
    public class PlayerStatusHandler : IPlayerStatusHandler
    {
        private readonly IMudProvider _mudProvider;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly ILogger<PlayerStatusHandler> _logger;
        private readonly IRecurringQueue _recurringQueue;
        private readonly IMediatorHandler _bus;
        private readonly IMudOnlineProvider _mudOnlineProvider;

        public PlayerStatusHandler(
            IMudProvider mudProvider, 
            IPlayerDomainService playerDomainService, 
            ILogger<PlayerStatusHandler> logger, 
            IRecurringQueue recurringQueue,
            IMudOnlineProvider mudOnlineProvider,
            IMediatorHandler bus)
        {
            _mudProvider = mudProvider;
            _playerDomainService = playerDomainService;
            _logger = logger;
            _recurringQueue = recurringQueue;
            _bus = bus;
            _mudOnlineProvider = mudOnlineProvider;
        }
        public async Task Execute(PlayerStatusModel model)
        {
            int playerId = model.PlayerId;
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                _logger.LogDebug($"player == null");
                return;
            }

            var online = await _mudOnlineProvider.GetPlayerOnline(playerId);
            if (online == null)
            {
                await _recurringQueue.Remove<PlayerStatusModel>(playerId);
                return;
            }

            if (player.Status!= model.Status)
            {
                await _recurringQueue.Remove<PlayerStatusModel>(playerId);
                return;
            }

          
            //TODO
            switch (model.Status)
            {
                case PlayerStatusEnum.伐木:
                    
                    break;


            }

            await _mudProvider.ShowMessage(playerId, $"你正在{model.Status}。。。");
        }
    }
}
