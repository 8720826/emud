using Emprise.Domain.Core.Data;
using Emprise.Domain.Npc.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Emprise.Domain.Npc.Services
{
    public class NpcScriptDomainService : INpcScriptDomainService
    {
        private readonly IRepository<NpcScriptEntity> _npcScriptRepository;

        public NpcScriptDomainService(IRepository<NpcScriptEntity> npcScriptRepository)
        {
            _npcScriptRepository = npcScriptRepository;
        }

        public async Task<List<NpcScriptEntity>> Query(Expression<Func<NpcScriptEntity, bool>> where)
        {
            var query =  await _npcScriptRepository.GetAll(where);
            return query.ToList();
        }

        public async Task<NpcScriptEntity> Get(Expression<Func<NpcScriptEntity, bool>> where)
        {
            return await _npcScriptRepository.Get(where);
        }

        public async Task<NpcScriptEntity> Get(int id)
        {
            return await _npcScriptRepository.Get(id);
        }

        public async Task Add(NpcScriptEntity user)
        {
            await _npcScriptRepository.Add(user);
        }

        public async Task Update(NpcScriptEntity user)
        {
             await _npcScriptRepository.Update(user);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
