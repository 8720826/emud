using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Npc.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Npc.Services
{
    public interface IScriptCommandDomainService : IBaseService
    {
        Task<List<ScriptCommandEntity>> Query(Expression<Func<ScriptCommandEntity, bool>> where);

        Task<ScriptCommandEntity> Get(Expression<Func<ScriptCommandEntity, bool>> where);

        Task<ScriptCommandEntity> Get(int id);

        Task Add(ScriptCommandEntity entity);

        Task Update(ScriptCommandEntity entity);
    }
}
