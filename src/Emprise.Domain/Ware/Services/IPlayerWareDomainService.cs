using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Ware.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Ware.Services
{

    public interface IPlayerWareDomainService : IBaseService
    {
        Task<PlayerWareEntity> Get(Expression<Func<PlayerWareEntity, bool>> where);

        Task<List<PlayerWareEntity>> GetAll(int playerId);

        Task<PlayerWareEntity> Get(int id);

        Task Add(PlayerWareEntity entity);

        Task Update(PlayerWareEntity entity);
    }
}
