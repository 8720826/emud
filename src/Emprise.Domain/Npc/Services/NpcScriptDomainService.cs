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
        private readonly IRepository<ScriptEntity> _npcScriptRepository;

        public NpcScriptDomainService(IRepository<ScriptEntity> npcScriptRepository)
        {
            _npcScriptRepository = npcScriptRepository;
        }

        public async Task<List<ScriptEntity>> Query(Expression<Func<ScriptEntity, bool>> where)
        {
            var query =  await _npcScriptRepository.GetAll(where);
            return query.ToList();
        }

        public async Task<ScriptEntity> Get(Expression<Func<ScriptEntity, bool>> where)
        {
            return await _npcScriptRepository.Get(where);
        }

        public async Task<ScriptEntity> Get(int id)
        {
            return await _npcScriptRepository.Get(id);
        }

        public async Task Add(ScriptEntity user)
        {
            await _npcScriptRepository.Add(user);
        }

        public async Task Update(ScriptEntity user)
        {
             await _npcScriptRepository.Update(user);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
