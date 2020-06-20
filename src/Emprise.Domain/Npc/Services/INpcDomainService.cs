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
    public interface INpcDomainService : IBaseService
    {
        Task<IQueryable<NpcEntity>> GetAll();

        Task<NpcEntity> Get(Expression<Func<NpcEntity, bool>> where);

        Task<NpcEntity> Get(int id);

        Task Add(NpcEntity npc);

        Task Update(NpcEntity npc);

        Task Delete(NpcEntity npc);
    }
}
