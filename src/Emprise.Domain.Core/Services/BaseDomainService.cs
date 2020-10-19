using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Core.Services
{
    public class BaseDomainService<TEntity>  where TEntity : class
    {
        private readonly IRepository<TEntity> _repository;
        private readonly IMemoryCache _cache;
        private readonly IMediatorHandler _bus;


        public BaseDomainService(IRepository<TEntity> repository, IMemoryCache cache, IMediatorHandler bus)
        {
            _repository = repository;
            _cache = cache;
            _bus = bus;
        }

        public async Task<IQueryable<TEntity>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<IQueryable<TEntity>> GetAllFromCache()
        {
            var key = $"{typeof(TEntity).Name}_List";
            return await _cache.GetOrCreateAsync(key, async p => {
                p.SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheKey.ExpireMinutes));
                return await _repository.GetAll();
            });
        }

        public async Task<TEntity> Get(Expression<Func<TEntity, bool>> where)
        {
            return await _repository.Get(where);
        }

        public async Task<TEntity> Get(int id)
        {
            return await _repository.Get(id);
        }

        public async Task<TEntity> GetFromCache(int id)
        {
            var key = $"{typeof(TEntity).Name}_{id}";
            return await _cache.GetOrCreateAsync(key, async p => {
                p.SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheKey.ExpireMinutes));
                return await _repository.Get(id);
            });
        }

        public async Task Add(TEntity entity)
        {
            await _repository.Add(entity);
        }

        public async Task Update(TEntity entity)
        {
            await _repository.Update(entity);
            await _bus.RaiseEvent(new EntityUpdatedEvent<TEntity>(entity)).ConfigureAwait(false);
        }

        public async Task Delete(TEntity entity)
        {
            await _repository.Remove(entity);
            await _bus.RaiseEvent(new EntityUpdatedEvent<TEntity>(entity)).ConfigureAwait(false);
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
