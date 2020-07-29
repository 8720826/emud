using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Interfaces.Ioc;
using Emprise.Domain.Player.Services;
using Emprise.MudServer.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.MudServer.JobHandlers
{
    public interface IPlayerLevelUpHandler : IScoped
    {
        Task Execute(int playerId);
    }
    public class PlayerLevelUpHandler: IPlayerLevelUpHandler
    {
        private readonly IMudProvider _mudProvider;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly ILogger<PlayerLevelUpHandler> _logger;
        private readonly IRecurringQueue _recurringQueue;
        private readonly IMediatorHandler _bus;
        private readonly IMudOnlineProvider _mudOnlineProvider;


        public PlayerLevelUpHandler(
            IMudProvider mudProvider,
            IPlayerDomainService playerDomainService,
            ILogger<PlayerLevelUpHandler> logger,
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

        public async Task Execute(int playerId)
        {

            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                _logger.LogDebug($"player == null");
                return;
            }

            var level = player.Level;
            var levelUpExp = (level * level * level + 60) / 5 * (level * 2 + 60) - 600;

            if (levelUpExp <= player.Exp)
            {
                player.Level += 1;
                player.Point += 1;
                await _playerDomainService.Update(player);

                await _bus.RaiseEvent(new PlayerStatusChangedEvent(player)).ConfigureAwait(false);

                await _mudProvider.ShowMessage(player.Id, $"恭喜你升级到 Lv{ player.Level }。");
            }


        }
    }
}
