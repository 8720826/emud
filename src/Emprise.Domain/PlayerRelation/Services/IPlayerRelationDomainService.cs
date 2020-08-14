using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.PlayerRelation.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.PlayerRelation.Services
{
    public interface  IPlayerRelationDomainService : IBaseService
    {
        Task<IQueryable<PlayerRelationEntity>> GetAll();

        Task<PlayerRelationEntity> Get(Expression<Func<PlayerRelationEntity, bool>> where);

        Task<PlayerRelationEntity> Get(int id);

        Task Add(PlayerRelationEntity player);

        Task Update(PlayerRelationEntity player);

        Task Delete(PlayerRelationEntity player);
    }
}
