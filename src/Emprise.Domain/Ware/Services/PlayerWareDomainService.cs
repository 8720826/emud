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
    public class PlayerWareDomainService : BaseDomainService<PlayerWareEntity>, IPlayerWareDomainService
    {
        private readonly IRepository<PlayerWareEntity> _scriptRepository;

        public PlayerWareDomainService(IRepository<PlayerWareEntity> scriptRepository, IMemoryCache cache, IMediatorHandler bus) : base(scriptRepository, cache, bus)
        {
            _scriptRepository = scriptRepository;
        }


        public async Task<List<PlayerWareEntity>> GetAll(int playerId)
        {
            var query = await _scriptRepository.GetAll(x => x.PlayerId == playerId);

            return query.ToList();
        }


    }
}
