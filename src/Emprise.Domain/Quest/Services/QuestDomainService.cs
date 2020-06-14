using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Quest.Entity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Emprise.Domain.Quest.Services
{
    public class QuestDomainService : IQuestDomainService
    {
        private readonly IRepository<QuestEntity> _questRepository;
        private readonly IMemoryCache _cache;
        private readonly IMediatorHandler _bus;
        private readonly ILogger<QuestDomainService> _logger;
        public QuestDomainService(IRepository<QuestEntity> questRepository, IMemoryCache cache, IMediatorHandler bus, ILogger<QuestDomainService> logger)
        {
            _questRepository = questRepository;
            _cache = cache;
            _bus = bus;
            _logger = logger;
        }


        public async Task<QuestEntity> Get(Expression<Func<QuestEntity, bool>> where)
        {
            return await _questRepository.Get(where);
        }

        public async Task<IQueryable<QuestEntity>> GetAll()
        {

            return await _questRepository.GetAll();
        }

        public async Task<QuestEntity> Get(int id)
        {
            var key = string.Format(CacheKey.Quest, id);
            return await _cache.GetOrCreateAsync(key, async p => {
                p.SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheKey.ExpireMinutes));
                return await _questRepository.Get(id);
            });
        }

        public async Task Add(QuestEntity quest)
        {
            await _questRepository.Add(quest);
            await _bus.RaiseEvent(new EntityInsertedEvent<QuestEntity>(quest)).ConfigureAwait(false);
        }

        public async Task Update(QuestEntity quest)
        {
            await _questRepository.Update(quest);
            await _bus.RaiseEvent(new EntityUpdatedEvent<QuestEntity>(quest)).ConfigureAwait(false);
        }

        public async Task Delete(QuestEntity quest)
        {
            await _questRepository.Remove(quest);
            await _bus.RaiseEvent(new EntityDeletedEvent<QuestEntity>(quest)).ConfigureAwait(false);
        }




        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
