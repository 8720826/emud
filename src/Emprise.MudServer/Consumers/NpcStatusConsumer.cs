using DotNetCore.CAP;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Interfaces.Ioc;
using Emprise.MudServer.Handles;
using Emprise.MudServer.Queues;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.MudServer.Consumers
{
    public interface INpcStatusConsumer : ITransient
    {
        Task<bool> NpcStatusQueueConsumer(NpcStatusQueue message);



    }

    public class NpcStatusConsumer : BaseConsumer, INpcStatusConsumer, ICapSubscribe
    {
        private readonly ILogger<NpcStatusConsumer> _logger;
        private readonly INpcStatusHandler _npcStatusHandler;
        private readonly IMudProvider _mudProvider;

        public NpcStatusConsumer(
            INpcStatusHandler npcStatusHandler,
            ILogger<NpcStatusConsumer> logger,
            IUnitOfWork uow,
            IMudProvider mudProvider,
            IRedisDb redisDb) : base(uow, redisDb)
        {
            _logger = logger;
            _npcStatusHandler = npcStatusHandler;
            _mudProvider = mudProvider;
        }

        [CapSubscribe("NpcStatusQueue")]
        public async Task<bool> NpcStatusQueueConsumer(NpcStatusQueue message)
        {
            _logger.LogDebug($"Consumer Get Queue {JsonConvert.SerializeObject(message)} ready");

            await _npcStatusHandler.Execute(message.Status);

            if (await Commit())
            {
                //await _bus.RaiseEvent(new ReceivedEmailEvent(playerId)).ConfigureAwait(false);
            }

            return true;
        }


    }
}
