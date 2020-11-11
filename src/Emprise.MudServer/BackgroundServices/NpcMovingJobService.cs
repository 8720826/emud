using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Npc.Services;
using Emprise.Domain.Room.Services;
using Emprise.MudServer.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emprise.MudServer.BackgroundServices
{
     
    public class NpcMovingJobService : BackgroundService
    {
        private readonly ILogger<NpcMovingJobService> _logger;
        private IServiceProvider _services;
        public NpcMovingJobService(IServiceProvider services, ILogger<NpcMovingJobService> logger)
        {
            _services = services;
            _logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await DoWork();
                await Task.Delay(1000* 30, stoppingToken);
            }
        }

        private async Task DoWork()
        {
            using (var scope = _services.CreateScope())
            {
                var _npcDomainService = scope.ServiceProvider.GetRequiredService<INpcDomainService>();
                var _redisDb = scope.ServiceProvider.GetRequiredService<IRedisDb>();
                var _mudProvider = scope.ServiceProvider.GetRequiredService<IMudProvider>();
                var _roomDomainService = scope.ServiceProvider.GetRequiredService<IRoomDomainService>();
                var _bus = scope.ServiceProvider.GetRequiredService<IMediatorHandler>();


                var npcs = await _npcDomainService.GetAllFromCache();
                var npc = npcs.Where(x => x.CanMove && !x.IsDead && x.IsEnable).OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                if (npc == null)
                {
                    return;
                }

                var npcFightingPlayerId = await _redisDb.StringGet<int>(string.Format(RedisKey.NpcFighting, npc.Id));
                if (npcFightingPlayerId > 0)
                {
                    return;
                }

                var roomOut = await _roomDomainService.Get(npc.RoomId);
                if (roomOut == null)
                {
                    return;
                }

                var roomOutId = npc.RoomId;

                var roomIds = new List<int> { roomOut.East, roomOut.West, roomOut.South, roomOut.North }.Where(x => x > 0).ToList();
                if (roomIds.Count == 0)
                {
                    return;
                }

                var roomInId = roomIds.OrderBy(x => Guid.NewGuid()).First();
                var roomIn = await _roomDomainService.Get(roomInId);
                if (roomIn == null)
                {
                    return;
                }

                npc.RoomId = roomInId;
                await _npcDomainService.Update(npc);

                await _bus.RaiseEvent(new NpcMovedEvent(npc, roomIn, roomOut));
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
