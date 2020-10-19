using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Services;
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
    public class ItemDropRateDomainService : BaseDomainService<ItemDropRateEntity>, IItemDropRateDomainService
    {
        private readonly IRepository<ItemDropRateEntity> _itemDropRateRepository;
        private readonly IMemoryCache _cache;
        private readonly IMediatorHandler _bus;

        public ItemDropRateDomainService(IRepository<ItemDropRateEntity> itemDropRateRepository, IMemoryCache cache, IMediatorHandler bus) : base(itemDropRateRepository, cache, bus)
        {
            _itemDropRateRepository = itemDropRateRepository;
            _cache = cache;
            _bus = bus;
        }



    }
}
