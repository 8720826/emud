using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Npc.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Npc.Services
{
    public interface INpcScriptDomainService : IBaseService
    {
        Task<IQueryable<NpcScriptEntity>> GetAll();

        Task<NpcScriptEntity> Get(Expression<Func<NpcScriptEntity, bool>> where);

        Task<NpcScriptEntity> Get(int id);

        Task Add(NpcScriptEntity entity);

        Task Update(NpcScriptEntity entity);

        Task Delete(NpcScriptEntity entity);
    }
}
