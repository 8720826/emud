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
    /// <summary>
    /// 检测用户是否在线
    /// </summary>
    public class OnlineCheckJobService : BackgroundService
    {
        private IServiceProvider _services;
        public OnlineCheckJobService(IServiceProvider services)
        {
            _services = services;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await DoWork();
                await Task.Delay(1000*60*30, stoppingToken);
            }
        }

        private async Task DoWork()
        {
            using (var scope = _services.CreateScope())
            {
                var mudOnlineProvider = scope.ServiceProvider.GetRequiredService<IMudOnlineProvider>();
                await mudOnlineProvider.CheckOnline().ConfigureAwait(false);
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
