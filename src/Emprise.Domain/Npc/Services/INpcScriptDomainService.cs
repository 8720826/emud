using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Npc.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Npc.Services
{
    public interface INpcScriptDomainService : IBaseService
    {
        Task<List<ScriptEntity>> Query(Expression<Func<ScriptEntity, bool>> where);

        Task<ScriptEntity> Get(Expression<Func<ScriptEntity, bool>> where);

        Task<ScriptEntity> Get(int id);

        Task Add(ScriptEntity entity);

        Task Update(ScriptEntity entity);
    }
}
