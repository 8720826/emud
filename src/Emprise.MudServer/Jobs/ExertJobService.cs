using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Bus.Models;
using Emprise.Domain.Core.Interfaces;
using Emprise.MudServer.Handles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emprise.MudServer.Jobs
{
    /// <summary>
    /// 打坐
    /// </summary>
    public class ExertJobService : BackgroundService
    {
        private IServiceProvider _services;
        public ExertJobService(IServiceProvider services)
        {
            _services = services;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await DoWork();
                await Task.Delay(50000, stoppingToken);
            }      
        }

        private async Task DoWork()
        {
            using (var scope = _services.CreateScope())
            {
                var queue = scope.ServiceProvider.GetRequiredService<IRecurringQueue>();
                var handle = scope.ServiceProvider.GetRequiredService<IExertHandler>();

                var msgs = await queue.Subscribe<ExertModel>();
                if (msgs == null || msgs.Count == 0)
                {
                    return;
                }

                foreach (var msg in msgs)
                {
                   await  handle.Execute(msg.Key, msg.Value).ConfigureAwait(false);
                }

            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
