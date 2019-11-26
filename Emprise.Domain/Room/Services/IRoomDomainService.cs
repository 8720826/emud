using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Room.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Room.Services
{
    public interface IRoomDomainService : IBaseService
    {
        Task<RoomEntity> Get(Expression<Func<RoomEntity, bool>> where);

        Task<List<RoomEntity>> GetAll(Expression<Func<RoomEntity, bool>> where);

        Task<RoomEntity> Get(int id);

        Task Add(RoomEntity user);

        Task Update(RoomEntity user);
    }
}
