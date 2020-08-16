using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Npc.Entity;
using Emprise.Domain.Quest.Services;
using Emprise.Domain.Skill.Entity;
using Emprise.MudServer.Events;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emprise.MudServer.EventHandlers
{
    public  class SkillEventHandler :
        INotificationHandler<EntityUpdatedEvent<SkillEntity>>,
        INotificationHandler<EntityInsertedEvent<SkillEntity>>,
        INotificationHandler<EntityDeletedEvent<SkillEntity>>
    {
        private readonly IMudProvider _mudProvider;
        private readonly ILogger<SkillEventHandler> _logger; 
        private readonly IRedisDb _redisDb;
        private readonly IMemoryCache _cache;

        public SkillEventHandler(ILogger<SkillEventHandler> logger, 
            IMudProvider mudProvider, 
            IMemoryCache cache,
            IRedisDb redisDb)
        {
            _logger = logger;
            _mudProvider = mudProvider;
            _redisDb = redisDb;
            _cache = cache;
        }



        public async Task Handle(EntityUpdatedEvent<SkillEntity> message, CancellationToken cancellationToken)
        {
            var key = string.Format(CacheKey.Skill, message.Entity.Id);
            await Task.Run(() => {
                _cache.Remove(key);
  
                _cache.Remove(CacheKey.BaseSkills);
            });
        }

        public async Task Handle(EntityInsertedEvent<SkillEntity> message, CancellationToken cancellationToken)
        {
            var key = string.Format(CacheKey.Skill, message.Entity.Id);
            await Task.Run(() => {
                _cache.Set(key, message.Entity);

                _cache.Remove(CacheKey.BaseSkills);
            });
        }

        public async Task Handle(EntityDeletedEvent<SkillEntity> message, CancellationToken cancellationToken)
        {
            var key = string.Format(CacheKey.Skill, message.Entity.Id);
            await Task.Run(() => {
                _cache.Remove(key);

                _cache.Remove(CacheKey.BaseSkills);
            });
        }

    }
}
