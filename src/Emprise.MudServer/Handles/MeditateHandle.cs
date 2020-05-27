using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Bus.Models;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Player.Services;
using Emprise.MudServer.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.MudServer.Handles
{
    public class MeditateHandle : IMeditateHandle
    {
        private readonly IMudProvider _mudProvider;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly ILogger<MeditateHandle> _logger;
        private readonly IRecurringQueue _recurringQueue;
        private readonly IMediatorHandler _bus;

        public MeditateHandle(IMudProvider mudProvider, IPlayerDomainService playerDomainService, ILogger<MeditateHandle> logger, IRecurringQueue recurringQueue, IMediatorHandler bus)
        {
            _mudProvider = mudProvider;
            _playerDomainService = playerDomainService;
            _logger = logger;
            _recurringQueue = recurringQueue;
            _bus = bus;
        }
        public async Task Execute(int playerId, MeditateModel model)
        {
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                _logger.LogDebug($"player == null");
                return;
            }

            if(player.Status!= PlayerStatusEnum.打坐)
            {
                _logger.LogDebug($"player.Status={player.Status}");
                return;
            }

            Random rnd = new Random();
            if (rnd.Next(1, 100) > 30)
            {
                await _mudProvider.ShowMessage(playerId, "你正在打坐。。。");
            }
            else
            {
                await _recurringQueue.Remove<MeditateModel>(playerId);
                await _mudProvider.ShowMessage(playerId, "打坐完成");
                player.Status = PlayerStatusEnum.空闲;
                await _playerDomainService.Update(player);

                await _bus.RaiseEvent(new PlayerStatusChangedEvent(player)).ConfigureAwait(false);
            }
           
        }
    }
}
