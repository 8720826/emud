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
    public interface IRoomDomainService : IBaseService
    {
        Task<RoomEntity> Get(Expression<Func<RoomEntity, bool>> where);

        Task<IQueryable<RoomEntity>> GetAll();

        Task<RoomEntity> Get(int id);

        Task Add(RoomEntity user);

        Task Update(RoomEntity user);

        Task Delete(RoomEntity entity);
    }
}
