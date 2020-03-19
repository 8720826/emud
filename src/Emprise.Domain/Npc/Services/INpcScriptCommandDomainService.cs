using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Npc.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Npc.Services
{
    public interface INpcScriptCommandDomainService : IBaseService
    {
        Task<List<NpcScriptCommandEntity>> Query(Expression<Func<NpcScriptCommandEntity, bool>> where);

        Task<NpcScriptCommandEntity> Get(Expression<Func<NpcScriptCommandEntity, bool>> where);

        Task<NpcScriptCommandEntity> Get(int id);

        Task Add(NpcScriptCommandEntity entity);

        Task Update(NpcScriptCommandEntity entity);
    }
}
