using Emprise.Domain.Common.Modes;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Quest.Entity;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emprise.MudServer.EventHandlers
{
    public class PlayerQuestEventHandler :
        INotificationHandler<EntityUpdatedEvent<PlayerQuestEntity>>,
        INotificationHandler<EntityInsertedEvent<PlayerQuestEntity>>,
        INotificationHandler<EntityDeletedEvent<PlayerQuestEntity>>
    {
        private readonly IMemoryCache _cache;
        public PlayerQuestEventHandler(IMemoryCache cache)
        {
            _cache = cache;
        }


        public async Task Handle(EntityUpdatedEvent<PlayerQuestEntity> message, CancellationToken cancellationToken)
        {
            var key = string.Format(CacheKey.PlayerQuestList, message.Entity.PlayerId);
            await Task.Run(() => {
                _cache.Remove(key);
            });
        }

        public async Task Handle(EntityInsertedEvent<PlayerQuestEntity> message, CancellationToken cancellationToken)
        {
            var key = string.Format(CacheKey.PlayerQuestList, message.Entity.PlayerId);
            await Task.Run(() => {
                _cache.Remove(key);
            });
        }

        public async Task Handle(EntityDeletedEvent<PlayerQuestEntity> message, CancellationToken cancellationToken)
        {
            var key = string.Format(CacheKey.PlayerQuestList, message.Entity.PlayerId);
            await Task.Run(() => {
                _cache.Remove(key);
            });
        }
    }
}
