using Emprise.Domain.Common.Modes;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Room.Entity;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emprise.Domain.Room.EventHandlers
{
    public  class RoomEventHandler :
        INotificationHandler<EntityUpdatedEvent<RoomEntity>>,
        INotificationHandler<EntityInsertedEvent<RoomEntity>>,
        INotificationHandler<EntityDeletedEvent<RoomEntity>>
    {
        private readonly IMemoryCache _cache;
        public RoomEventHandler(IMemoryCache cache)
        {
            _cache = cache;
        }


        public async Task Handle(EntityUpdatedEvent<RoomEntity> message, CancellationToken cancellationToken)
        {
            var key = string.Format(CacheKey.Room, message.Entity.Id);
            await Task.Run(() => {
                _cache.Remove(key);
            });
        }

        public async Task Handle(EntityInsertedEvent<RoomEntity> message, CancellationToken cancellationToken)
        {
            var key = string.Format(CacheKey.Room, message.Entity.Id);
            await Task.Run(() => {
                _cache.Set(key, message.Entity);
            });
        }

        public async Task Handle(EntityDeletedEvent<RoomEntity> message, CancellationToken cancellationToken)
        {
            var key = string.Format(CacheKey.Room, message.Entity.Id);
            await Task.Run(() => {
                _cache.Remove(key);
            });
        }
    }
}
