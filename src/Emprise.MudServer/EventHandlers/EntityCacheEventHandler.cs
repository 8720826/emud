using Emprise.Domain.Core.Events;
using Emprise.Domain.Email.Entity;
using Emprise.Domain.Skill.Entity;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emprise.MudServer.EventHandlers
{
    public class EntityCacheEventHandler :
        INotificationHandler<EntityUpdatedEvent<EmailEntity>>,
        INotificationHandler<EntityInsertedEvent<EmailEntity>>,
        INotificationHandler<EntityDeletedEvent<EmailEntity>>,


        INotificationHandler<EntityUpdatedEvent<SkillEntity>>,
        INotificationHandler<EntityInsertedEvent<SkillEntity>>,
        INotificationHandler<EntityDeletedEvent<SkillEntity>>

        
    {
        private readonly IMemoryCache _cache;

        public EntityCacheEventHandler(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task Handle(EntityUpdatedEvent<EmailEntity> message, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        public async Task Handle(EntityInsertedEvent<EmailEntity> message, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        public async Task Handle(EntityDeletedEvent<EmailEntity> message, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        public async Task Handle(EntityUpdatedEvent<SkillEntity> message, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        public async Task Handle(EntityInsertedEvent<SkillEntity> message, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        public async Task Handle(EntityDeletedEvent<SkillEntity> message, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}
