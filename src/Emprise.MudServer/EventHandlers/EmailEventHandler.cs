using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Email.Entity;
using Emprise.Domain.Email.Services;
using Emprise.MudServer.Events.EmailEvents;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emprise.MudServer.EventHandlers
{
    public class EmailEventHandler :
        INotificationHandler<EntityUpdatedEvent<EmailEntity>>,
        INotificationHandler<EntityInsertedEvent<EmailEntity>>,
        INotificationHandler<EntityDeletedEvent<EmailEntity>>,
        INotificationHandler<ReceivedEmailEvent>,
        INotificationHandler<EmailStatusChangedEvent>,
        INotificationHandler<DeletedEmailEvent>


        

    {
        private readonly IMemoryCache _cache;
        private readonly IPlayerEmailDomainService _playerEmailDomainService;
        private readonly IMudProvider _mudProvider;
        private readonly IRedisDb _redisDb;

        public EmailEventHandler(IMemoryCache cache, 
            IPlayerEmailDomainService playerEmailDomainService,
            IRedisDb redisDb,
            IMudProvider mudProvider)
        {
            _cache = cache;
            _playerEmailDomainService = playerEmailDomainService;
            _mudProvider = mudProvider;
            _redisDb = redisDb;
        }


        public async Task Handle(EntityUpdatedEvent<EmailEntity> message, CancellationToken cancellationToken)
        {

        }

        public async Task Handle(EntityInsertedEvent<EmailEntity> message, CancellationToken cancellationToken)
        {

        }

        public async Task Handle(EntityDeletedEvent<EmailEntity> message, CancellationToken cancellationToken)
        {

        }

        public async Task Handle(ReceivedEmailEvent message, CancellationToken cancellationToken)
        {
            var playerId = message.PlayerId;

            var count = await _playerEmailDomainService.GetUnreadCount(playerId);

            await _mudProvider.UpdateUnreadEmailCount(playerId, count);

            string key = string.Format(RedisKey.UnreadEmailCount, playerId);
            await _redisDb.StringSet(key, count, DateTime.Now.AddMinutes(60));

        }

        public async Task Handle(EmailStatusChangedEvent message, CancellationToken cancellationToken)
        {
            var playerId = message.PlayerId;

            var count = await _playerEmailDomainService.GetUnreadCount(playerId);

            await _mudProvider.UpdateUnreadEmailCount(playerId, count);

            string key = string.Format(RedisKey.UnreadEmailCount, playerId);
            await _redisDb.StringSet(key, count, DateTime.Now.AddMinutes(60));

        }

        public async Task Handle(DeletedEmailEvent message, CancellationToken cancellationToken)
        {
            var playerId = message.PlayerId;

            var count = await _playerEmailDomainService.GetUnreadCount(playerId);

            await _mudProvider.UpdateUnreadEmailCount(playerId, count);

            string key = string.Format(RedisKey.UnreadEmailCount, playerId);
            await _redisDb.StringSet(key, count, DateTime.Now.AddMinutes(60));

        }
    }
}
