using DotNetCore.CAP;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Interfaces.Ioc;
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
    public interface IChatConsumer:ITransient
    {
        Task<bool> SaveChatLogQueueConsumer(SaveChatLogQueue message);

    }

    public class ChatConsumer : BaseConsumer, IChatConsumer, ICapSubscribe
    {
        private readonly ILogger<ChatConsumer> _logger;
        private readonly IChatLogDomainService _chatLogDomainService;

        public ChatConsumer(
            IChatLogDomainService chatLogDomainService,
            ILogger<ChatConsumer> logger, IUnitOfWork uow) :base(uow)
        {
            _logger = logger;
            _chatLogDomainService = chatLogDomainService;
        }

        [CapSubscribe("SaveChatLogQueue")]
        public async Task<bool> SaveChatLogQueueConsumer(SaveChatLogQueue message)
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
