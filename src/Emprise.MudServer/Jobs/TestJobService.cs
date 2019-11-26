using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Interfaces;
using Emprise.MudServer.Jobs.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emprise.MudServer.Jobs
{
    public class TestJobService : BackgroundService
    {
        private IServiceProvider _services;
        public TestJobService(IServiceProvider services)
        {
            _services = services;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await DoWork();
                await Task.Delay(500, stoppingToken);
            }
        }

        private async Task DoWork()
        {
            using (var scope = _services.CreateScope())
            {
                var queue = scope.ServiceProvider.GetRequiredService<IDelayedQueue>();
                var _jobProvider = scope.ServiceProvider.GetRequiredService<IMudProvider>();
                var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

                var msgs = await queue.Subscribe<MessageModel>();
                if (msgs == null || msgs.Count == 0)
                {
                    return;
                }

                foreach (var msg in msgs)
                {
                   await  _jobProvider.ShowMessage(msg.PlayerId,$"你说：{msg.Content}").ConfigureAwait(false);
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
