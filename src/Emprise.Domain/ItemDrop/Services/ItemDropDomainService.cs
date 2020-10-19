using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Models;
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
    public class ItemDropDomainService: BaseDomainService<ItemDropEntity>, IItemDropDomainService
    {
        private readonly IRepository<ItemDropEntity> _itemDropRepository;
        private readonly IMemoryCache _cache;
        private readonly IMediatorHandler _bus;

        public ItemDropDomainService(IRepository<ItemDropEntity> itemDropRepository, IMemoryCache cache, IMediatorHandler bus) : base(itemDropRepository, cache, bus)
        {
            _itemDropRepository = itemDropRepository;
            _cache = cache;
            _bus = bus;
        }

    }
}
