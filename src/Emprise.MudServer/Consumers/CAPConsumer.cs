using DotNetCore.CAP;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Log.Entity;
using Emprise.Domain.Log.Services;
using Emprise.MudServer.Queues;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.MudServer.Consumers
{
    public interface ICAPConsumer
    {
        Task<bool> TestQueueConsumer(SendMessageQueue message);

    }

    public class CAPConsumer : BaseConsumer, ICAPConsumer, ICapSubscribe
    {
        private readonly ILogger<CAPConsumer> _logger;
        private readonly IChatLogDomainService _chatLogDomainService;

        public CAPConsumer(
            IChatLogDomainService chatLogDomainService,
            ILogger<CAPConsumer> logger, IUnitOfWork uow) :base(uow)
        {
            _logger = logger;
            _chatLogDomainService = chatLogDomainService;
        }

        [CapSubscribe("SendMessageQueue")]
        public async Task<bool> TestQueueConsumer(SendMessageQueue message)
        {
            _logger.LogDebug($"Consumer Get Queue {JsonConvert.SerializeObject(message)} ready");

            var playerId = message.PlayerId;
            var content = message.Content;

            await _chatLogDomainService.Add(new ChatLogEntity
            {
                PlayerId = playerId,
                Content = content,
                PostDate = DateTime.Now
            });


            await Commit();

            return true;
        }
    }
}
