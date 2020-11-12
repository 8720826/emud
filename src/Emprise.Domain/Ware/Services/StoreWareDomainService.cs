using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Services;
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

    public class StoreWareDomainService : BaseDomainService<StoreWareEntity>, IStoreWareDomainService
    {
        private readonly IRepository<StoreWareEntity> _wareRepository;

        public StoreWareDomainService(IRepository<StoreWareEntity> wareRepository, IMemoryCache cache, IMediatorHandler bus) : base(wareRepository, cache, bus)
        {
            _wareRepository = wareRepository;

        }



    }
}
