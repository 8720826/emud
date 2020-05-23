using Emprise.Domain.Common.Modes;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Ware.Entity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Ware.Services
{

    public class WareDomainService : IWareDomainService
    {
        private readonly IRepository<WareEntity> _scriptRepository;
        private readonly IMemoryCache _cache;
        private readonly IMediatorHandler _bus;

        public WareDomainService(IRepository<WareEntity> scriptRepository, IMemoryCache cache, IMediatorHandler bus)
        {
            _scriptRepository = scriptRepository;
            _cache = cache;
            _bus = bus;
        }


        public async Task<WareEntity> Get(Expression<Func<WareEntity, bool>> where)
        {
            return await _scriptRepository.Get(where);
        }

        public async Task<List<WareEntity>> GetAll(Expression<Func<WareEntity, bool>> where)
        {
            var query = await _scriptRepository.GetAll(where);

            return query.ToList();
        }

        public async Task<WareEntity> Get(int id)
        {
            var key = string.Format(CacheKey.Ware, id);
            return await _cache.GetOrCreateAsync(key, async p => {
                p.SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheKey.ExpireMinutes));
                return await _scriptRepository.Get(id);
            });
        }

        public async Task Add(WareEntity entity)
        {
            await _scriptRepository.Add(entity);
        }

        public async Task Update(WareEntity entity)
        {
            await _scriptRepository.Update(entity);
            await _bus.RaiseEvent(new EntityUpdatedEvent<WareEntity>(entity)).ConfigureAwait(false);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
