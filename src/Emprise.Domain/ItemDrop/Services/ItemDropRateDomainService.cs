using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Events;
using Emprise.Domain.ItemDrop.Entity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.ItemDrop.Services
{
    public class ItemDropRateDomainService : IItemDropRateDomainService
    {
        private readonly IRepository<ItemDropRateEntity> _itemDropRateRepository;
        private readonly IMemoryCache _cache;
        private readonly IMediatorHandler _bus;

        public ItemDropRateDomainService(IRepository<ItemDropRateEntity> itemDropRateRepository, IMemoryCache cache, IMediatorHandler bus)
        {
            _itemDropRateRepository = itemDropRateRepository;
            _cache = cache;
            _bus = bus;
        }


        public async Task<ItemDropRateEntity> Get(Expression<Func<ItemDropRateEntity, bool>> where)
        {
            return await _itemDropRateRepository.Get(where);
        }


        public async Task<ItemDropRateEntity> Get(int id)
        {
            return await _itemDropRateRepository.Get(id);
        }

        public async Task Add(ItemDropRateEntity entity)
        {
            await _itemDropRateRepository.Add(entity);
        }

        public async Task<IQueryable<ItemDropRateEntity>> GetAll()
        {
            return await _itemDropRateRepository.GetAll();
        }

        public async Task Update(ItemDropRateEntity entity)
        {
            await _itemDropRateRepository.Update(entity);
            await _bus.RaiseEvent(new EntityUpdatedEvent<ItemDropRateEntity>(entity)).ConfigureAwait(false);
        }

        public async Task Delete(ItemDropRateEntity entity)
        {
            await _itemDropRateRepository.Remove(entity);
            await _bus.RaiseEvent(new EntityDeletedEvent<ItemDropRateEntity>(entity)).ConfigureAwait(false);
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
