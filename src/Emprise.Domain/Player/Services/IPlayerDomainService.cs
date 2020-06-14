using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Player.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Player.Services
{
    public interface IPlayerDomainService : IBaseService
    {
        Task<IQueryable<PlayerEntity>> GetAll();


        Task<PlayerEntity> Get(Expression<Func<PlayerEntity, bool>> where);

        Task<PlayerEntity> Get(int id);

        Task Add(PlayerEntity player);

        Task Update(PlayerEntity player);

        Task Delete(PlayerEntity player);

        Task<PlayerEntity> GetUserPlayer(int userId);
    }
}
