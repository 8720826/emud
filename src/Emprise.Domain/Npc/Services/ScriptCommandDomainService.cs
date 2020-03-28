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
    public class ScriptCommandDomainService : IScriptCommandDomainService
    {
        private readonly IRepository<ScriptCommandEntity> _npcScriptRepository;

        public ScriptCommandDomainService(IRepository<ScriptCommandEntity> npcScriptRepository)
        {
            _npcScriptRepository = npcScriptRepository;
        }

        public async Task<List<ScriptCommandEntity>> Query(Expression<Func<ScriptCommandEntity, bool>> where)
        {
            var query =  await _npcScriptRepository.GetAll(where);
            return query.ToList();
        }

        public async Task<ScriptCommandEntity> Get(Expression<Func<ScriptCommandEntity, bool>> where)
        {
            return await _npcScriptRepository.Get(where);
        }

        public async Task<ScriptCommandEntity> Get(int id)
        {
            return await _npcScriptRepository.Get(id);
        }

        public async Task Add(ScriptCommandEntity user)
        {
            await _npcScriptRepository.Add(user);
        }

        public async Task Update(ScriptCommandEntity user)
        {
             await _npcScriptRepository.Update(user);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
