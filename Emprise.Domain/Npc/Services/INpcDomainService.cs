using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Npc.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Npc.Services
{
    public interface INpcDomainService : IBaseService
    {
        Task<List<NpcEntity>> Query(Expression<Func<NpcEntity, bool>> where);

        Task<NpcEntity> Get(Expression<Func<NpcEntity, bool>> where);

        Task<NpcEntity> Get(int id);

        Task Add(NpcEntity user);

        Task Update(NpcEntity user);
    }
}
