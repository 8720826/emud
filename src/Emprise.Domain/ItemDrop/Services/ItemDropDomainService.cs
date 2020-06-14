using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Models;
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
    public class ItemDropDomainService: IItemDropDomainService
    {
        private readonly IRepository<ItemDropEntity> _itemDropRepository;
        private readonly IMemoryCache _cache;
        private readonly IMediatorHandler _bus;

        public ItemDropDomainService(IRepository<ItemDropEntity> itemDropRepository, IMemoryCache cache, IMediatorHandler bus)
        {
            _itemDropRepository = itemDropRepository;
            _cache = cache;
            _bus = bus;
        }


        public async Task<ItemDropEntity> Get(Expression<Func<ItemDropEntity, bool>> where)
        {
            return await _itemDropRepository.Get(where);
        }


        public async Task<ItemDropEntity> Get(int id)
        {
            return await _itemDropRepository.Get(id);
        }

        public async Task Add(ItemDropEntity entity)
        {
            await _itemDropRepository.Add(entity);
        }

        public async Task<IQueryable<ItemDropEntity>> GetAll()
        {
            return await _itemDropRepository.GetAll();
        }

        public async Task Update(ItemDropEntity entity)
        {
            await _itemDropRepository.Update(entity);
            await _bus.RaiseEvent(new EntityUpdatedEvent<ItemDropEntity>(entity)).ConfigureAwait(false);
        }

        public async Task Delete(ItemDropEntity entity)
        {
            await _itemDropRepository.Remove(entity);
            await _bus.RaiseEvent(new EntityDeletedEvent<ItemDropEntity>(entity)).ConfigureAwait(false);
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
