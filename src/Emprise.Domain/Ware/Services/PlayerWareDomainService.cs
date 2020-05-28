using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
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
    public class PlayerWareDomainService : IPlayerWareDomainService
    {
        private readonly IRepository<PlayerWareEntity> _scriptRepository;
        private readonly IMemoryCache _cache;
        private readonly IMediatorHandler _bus;

        public PlayerWareDomainService(IRepository<PlayerWareEntity> scriptRepository, IMemoryCache cache, IMediatorHandler bus)
        {
            _scriptRepository = scriptRepository;
            _cache = cache;
            _bus = bus;
        }


        public async Task<PlayerWareEntity> Get(Expression<Func<PlayerWareEntity, bool>> where)
        {
            return await _scriptRepository.Get(where);
        }

        public async Task<List<PlayerWareEntity>> GetAll(int playerId)
        {
            var query = await _scriptRepository.GetAll(x => x.PlayerId == playerId);

            return query.ToList();
        }

        public async Task<PlayerWareEntity> Get(int id)
        {
            return await _scriptRepository.Get(id);
        }

        public async Task Add(PlayerWareEntity entity)
        {
            await _scriptRepository.Add(entity);
        }

        public async Task Update(PlayerWareEntity entity)
        {
            await _scriptRepository.Update(entity);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
