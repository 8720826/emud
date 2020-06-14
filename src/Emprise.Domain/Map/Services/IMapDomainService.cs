using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Map.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Map.Services
{
    public interface IMapDomainService : IBaseService
    {
        Task<IQueryable<MapEntity>> GetAll();

        Task<MapEntity> Get(int id);

        Task Add(MapEntity entity);

        Task Update(MapEntity entity);

        Task Delete(MapEntity entity);

        Task<int> GetRoomCount(int id);
    }
}
