using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Quest.Entity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Quest.Services
{
    public class PlayerQuestDomainService : IPlayerQuestDomainService
    {
        private readonly IRepository<PlayerQuestEntity> _questRepository;
        private readonly IMemoryCache _cache;
        private readonly IMediatorHandler _bus;
        private readonly ILogger<QuestDomainService> _logger;
        public PlayerQuestDomainService(IRepository<PlayerQuestEntity> questRepository, IMemoryCache cache, IMediatorHandler bus, ILogger<QuestDomainService> logger)
        {
            _questRepository = questRepository;
            _cache = cache;
            _bus = bus;
            _logger = logger;
        }

        public async Task<List<PlayerQuestEntity>> GetPlayerQuests(int playerId)
        {
            var key = string.Format(CacheKey.PlayerQuestList, playerId);
            return await _cache.GetOrCreateAsync(key, async p => {
                p.SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheKey.ExpireMinutes));
                return (await _questRepository.GetAll(x => x.PlayerId == playerId)).ToList();
            });
        }

        public async Task<PlayerQuestEntity> Get(Expression<Func<PlayerQuestEntity, bool>> where)
        {
            return await _questRepository.Get(where);
        }


        public async Task<PlayerQuestEntity> Get(int id)
        {
            return await _questRepository.Get(id);
        }

        public async Task Add(PlayerQuestEntity entity)
        {
            await _questRepository.Add(entity);
            await _bus.RaiseEvent(new EntityInsertedEvent<PlayerQuestEntity>(entity)).ConfigureAwait(false);
        }

        public async Task Update(PlayerQuestEntity entity)
        {
            await _questRepository.Update(entity);
            await _bus.RaiseEvent(new EntityUpdatedEvent<PlayerQuestEntity>(entity)).ConfigureAwait(false);
        }




        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
