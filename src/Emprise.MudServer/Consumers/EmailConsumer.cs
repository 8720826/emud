using DotNetCore.CAP;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Interfaces.Ioc;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Email.Entity;
using Emprise.Domain.Email.Services;
using Emprise.MudServer.Events.EmailEvents;
using Emprise.MudServer.Queues;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.MudServer.Consumers
{
    public interface IEmailConsumer : ITransient
    {
        Task<bool> ReceiveEmailQueueConsumer(ReceiveEmailQueue message);

    }

    public class EmailConsumer : BaseConsumer, IEmailConsumer, ICapSubscribe
    {
        private readonly ILogger<ChatConsumer> _logger;
        private readonly IEmailDomainService _emailDomainService;
        private readonly IPlayerEmailDomainService _playerEmailDomainService;
        private readonly IMediatorHandler _bus;
        private readonly IMudProvider _mudProvider;

        public EmailConsumer(
            IEmailDomainService emailDomainService,
            IPlayerEmailDomainService playerEmailDomainService,
            IMediatorHandler bus,
            ILogger<ChatConsumer> logger, 
            IUnitOfWork uow,
            IMudProvider mudProvider,
            IRedisDb redisDb) : base(uow, redisDb)
        {
            _logger = logger;
            _emailDomainService = emailDomainService;
            _playerEmailDomainService = playerEmailDomainService;
            _bus = bus;
            _mudProvider = mudProvider;
        }

        [CapSubscribe("ReceiveEmailQueue")]
        public async Task<bool> ReceiveEmailQueueConsumer(ReceiveEmailQueue message)
        {
            _logger.LogDebug($"Consumer Get Queue {JsonConvert.SerializeObject(message)} ready");

            var playerId = message.PlayerId;

            string key = string.Format(RedisKey.UnreadEmailCount, playerId);
            long ttl = await _redisDb.KeyTimeToLive(key);
            if (ttl > 0)
            {
                var unreadEmailCount = await _redisDb.StringGet<int>(key);
                await _mudProvider.UpdateUnreadEmailCount(playerId, unreadEmailCount);
                return false;
            }

            

            var emails = await _emailDomainService.GetMyEmails(playerId);

            var myReceivedEmails = await _playerEmailDomainService.GetMyReceivedEmails(playerId);

            var notReceivedEmails = emails.Where(x => !myReceivedEmails.Select(x => x.EmailId).Contains(x.Id)).ToList();

            foreach (var email in notReceivedEmails)
            {
                var playerEmail = new PlayerEmailEntity
                {
                    CreateDate = DateTime.Now,
                    ExpiryDate = email.ExpiryDate,
                    EmailId = email.Id,
                    Status = EmailStatusEnum.未读,
                    PlayerId = playerId
                };

                await _playerEmailDomainService.Add(playerEmail);
            }

            if (await Commit())
            {
                await _bus.RaiseEvent(new ReceivedEmailEvent(playerId)).ConfigureAwait(false);
            }

            return true;
        }
    }
}
