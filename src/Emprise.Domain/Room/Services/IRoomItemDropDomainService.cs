using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Room.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Room.Services
{
    public interface IRoomItemDropDomainService : IBaseService
    {
        Task<RoomItemDropEntity> Get(Expression<Func<RoomItemDropEntity, bool>> where);

        Task<IQueryable<RoomItemDropEntity>> GetAll();

        Task<RoomItemDropEntity> Get(int id);

        Task Add(RoomItemDropEntity entity);

        Task Update(RoomItemDropEntity entity);
    }
}
