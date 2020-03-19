using Emprise.Domain.Common.Modes;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Npc.Entity;
using Emprise.Domain.Room.Entity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Emprise.Domain.Script.Services
{
    public class ScriptDomainService : IScriptDomainService
    { 
        private readonly IRepository<NpcScriptEntity> _scriptRepository;
        private readonly IMemoryCache _cache;
        private readonly IMediatorHandler _bus;

        public ScriptDomainService(IRepository<NpcScriptEntity> scriptRepository, IMemoryCache cache, IMediatorHandler bus)
        {
            _scriptRepository = scriptRepository;
            _cache = cache;
            _bus = bus;
        }


        public async Task<NpcScriptEntity> Get(Expression<Func<NpcScriptEntity, bool>> where)
        {
            return await _scriptRepository.Get(where);
        }

        public async Task<List<NpcScriptEntity>> GetAll(Expression<Func<NpcScriptEntity, bool>> where)
        {
            var query = await _scriptRepository.GetAll(where);

            return query.ToList();
        }

        public async Task<NpcScriptEntity> Get(int id)
        {
            var key = string.Format(CacheKey.Script, id);
            return await _cache.GetOrCreateAsync(key, async p => {
                p.SetAbsoluteExpiration(TimeSpan.FromMinutes(60));
                return await Task.Run(async () =>
                {
                    return await _scriptRepository.Get(id);
                });
            });
        }

        public async Task Add(NpcScriptEntity entity)
        {
            await _scriptRepository.Add(entity);
        }

        public async Task Update(NpcScriptEntity entity)
        {
            await _scriptRepository.Update(entity);
            await _bus.RaiseEvent(new EntityUpdatedEvent<NpcScriptEntity>(entity)).ConfigureAwait(false);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
