using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Bus.Models;
using Emprise.Domain.Core.Interfaces;
using Emprise.MudServer.Handles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
    public class MeditateJobService : BackgroundService
    {
        public ILogger _logger;
        private IServiceProvider _services;
        public MeditateJobService(IServiceProvider services, ILogger<MeditateJobService> logger)
        {
            _services = services;
            _logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                //_logger.LogInformation(DateTime.Now.ToString() + "BackgroundService：启动");
                while (!stoppingToken.IsCancellationRequested)
                {
                    await DoWork();
                    await Task.Delay(500, stoppingToken);
                }
               // _logger.LogInformation(DateTime.Now.ToString() + "BackgroundService：停止");
            }
            catch (Exception ex)
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation(DateTime.Now.ToString() + "BackgroundService：异常" + ex.Message + ex.StackTrace);
                }
                else
                {
                    //_logger.LogInformation(DateTime.Now.ToString() + "BackgroundService：停止");
                }
            }
        }

        private async Task DoWork()
        {
            //_logger.LogDebug($"MeditateJobService DoWork");
            using (var scope = _services.CreateScope())
            {
                var queue = scope.ServiceProvider.GetRequiredService<IRecurringQueue>();
                var handle = scope.ServiceProvider.GetRequiredService<IMeditateHandle>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<MeditateJobService>>();
                var msgs = await queue.Subscribe<MeditateModel>();
                if (msgs == null || msgs.Count == 0)
                {
                  //  logger.LogDebug($"msgs == null || msgs.Count == 0");
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
            //_logger.LogDebug($"MeditateJobService StopAsync");
            return base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
