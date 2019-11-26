using Emprise.Application.User.Services;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Consumers;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Player.Commands;
using Emprise.Domain.Player.Events;
using Emprise.Infra.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emprise.Application.Common.Consumers
{
    public class Subscriber : BackgroundService
    {
        public IServiceProvider _services;

        private IQueueHandler _queue;
        private IChatProvider _mudProvider;
        private IMediatorHandler _bus;
        private IPlayerAppService  _playerAppService;
        public Subscriber(IServiceProvider services)
        {
            _services = services;
        }



        protected  override  Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _services.CreateScope())
            {
                _queue = scope.ServiceProvider.GetRequiredService<IQueueHandler>();
                _mudProvider = scope.ServiceProvider.GetRequiredService<IChatProvider>();
                _bus = scope.ServiceProvider.GetRequiredService<IMediatorHandler>();
                _playerAppService = scope.ServiceProvider.GetRequiredService<IPlayerAppService>();

                _queue.Subscribe("initGame", (channel, playerId) => {
                  


                    _playerAppService.InitGame(Convert.ToInt32(playerId));
                });


            }

            return Task.CompletedTask;

        }

    }
}
