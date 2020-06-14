using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Models;
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
        private readonly IRepository<WareEntity> _wareRepository;
        private readonly IMemoryCache _cache;
        private readonly IMediatorHandler _bus;

        public WareDomainService(IRepository<WareEntity> wareRepository, IMemoryCache cache, IMediatorHandler bus)
        {
            _wareRepository = wareRepository;
            _cache = cache;
            _bus = bus;
        }


        public async Task<WareEntity> Get(Expression<Func<WareEntity, bool>> where)
        {
            return await _wareRepository.Get(where);
        }


        public async Task<WareEntity> Get(int id)
        {
            var key = string.Format(CacheKey.Ware, id);
            return await _cache.GetOrCreateAsync(key, async p => {
                p.SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheKey.ExpireMinutes));
                return await _wareRepository.Get(id);
            });
        }

        public async Task Add(WareEntity entity)
        {
            await _wareRepository.Add(entity);
        }

        public async Task<IQueryable<WareEntity>> GetAll()
        {
            return await _wareRepository.GetAll();
        }

        public async Task Update(WareEntity entity)
        {
            await _wareRepository.Update(entity);
            await _bus.RaiseEvent(new EntityUpdatedEvent<WareEntity>(entity)).ConfigureAwait(false);
        }

        public async Task Delete(WareEntity entity)
        {
            await _wareRepository.Remove(entity);
            await _bus.RaiseEvent(new EntityDeletedEvent<WareEntity>(entity)).ConfigureAwait(false);
        }
        

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
