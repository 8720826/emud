using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Npc.Entity;
using Emprise.Domain.Quest.Services;
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
    public  class NpcEventHandler :
        INotificationHandler<EntityUpdatedEvent<NpcEntity>>,
        INotificationHandler<EntityInsertedEvent<NpcEntity>>,
        INotificationHandler<EntityDeletedEvent<NpcEntity>>,
        INotificationHandler<ChatWithNpcEvent>
    {
        private readonly IQuestDomainService _questDomainService;
        private readonly IPlayerQuestDomainService _playerQuestDomainService;
        private readonly IMudProvider _mudProvider;
        private readonly ILogger<NpcEventHandler> _logger; 
        private readonly IRedisDb _redisDb;
        private readonly IMemoryCache _cache;

        public NpcEventHandler(ILogger<NpcEventHandler> logger, 
            IMudProvider mudProvider, 
            IQuestDomainService questDomainService, 
            IPlayerQuestDomainService playerQuestDomainService,
            IMemoryCache cache,
            IRedisDb redisDb)
        {
            _logger = logger;
            _mudProvider = mudProvider;
            _questDomainService = questDomainService;
            _playerQuestDomainService = playerQuestDomainService;
            _redisDb = redisDb;
            _cache = cache;
        }

        public async Task Handle(EntityUpdatedEvent<NpcEntity> message, CancellationToken cancellationToken)
        {
            var key = string.Format(CacheKey.Npc, message.Entity.Id);

            _logger.LogInformation($"{key}");

            await Task.Run(() => {
                _cache.Remove(key);
            });
        }

        public async Task Handle(EntityInsertedEvent<NpcEntity> message, CancellationToken cancellationToken)
        {
            var key = string.Format(CacheKey.Npc, message.Entity.Id);
            await Task.Run(() => {
                _cache.Set(key, message.Entity);
            });
        }

        public async Task Handle(EntityDeletedEvent<NpcEntity> message, CancellationToken cancellationToken)
        {
            var key = string.Format(CacheKey.Npc, message.Entity.Id);
            await Task.Run(() => {
                _cache.Remove(key);
            });
        }

        public async Task Handle(ChatWithNpcEvent message, CancellationToken cancellationToken)
        {
            var playerId = message.PlayerId;
            var npcId = message.NpcId;


            await _redisDb.StringSet<int>(string.Format(RedisKey.ChatWithNpc, playerId, npcId), 1, DateTime.Now.AddDays(30));




        }
    }
}
