
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Queue.Models;
using Emprise.MudServer.Queues;
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
    /// 疗伤
    /// </summary>
    public class NpcStatusJobService : BackgroundService
    {
        private IServiceProvider _services;
        public NpcStatusJobService(IServiceProvider services)
        {
            _services = services;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await DoWork();
                await Task.Delay(100, stoppingToken);
            }      
        }

        private async Task DoWork()
        {
            using (var scope = _services.CreateScope())
            {
                var queue = scope.ServiceProvider.GetRequiredService<IRecurringQueue>();
                var handle = scope.ServiceProvider.GetRequiredService<IQueueHandler>();
                var msgs = await queue.Subscribe<NpcStatusModel>();
                if (msgs == null || msgs.Count == 0)
                {
                    return;
                }

                //发布到队列
                foreach (var msg in msgs)
                {
                    await handle.SendQueueMessage(new NpcStatusQueue(msg.Value));
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
