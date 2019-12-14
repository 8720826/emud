using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Room.Entity;
using Emprise.Domain.Script.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Script.Services
{
    public interface IScriptDomainService : IBaseService
    {
        Task<ScriptEntity> Get(Expression<Func<ScriptEntity, bool>> where);

        Task<List<ScriptEntity>> GetAll(Expression<Func<ScriptEntity, bool>> where);

        Task<ScriptEntity> Get(int id);

        Task Add(ScriptEntity entity);

        Task Update(ScriptEntity entity);
    }
}
