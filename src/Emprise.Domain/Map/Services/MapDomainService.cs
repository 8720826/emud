using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Services;
using Emprise.Domain.Map.Entity;
using Emprise.Domain.Room.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Map.Services
{
    public class MapDomainService : BaseDomainService<MapEntity>, IMapDomainService
    {
        private readonly IRepository<MapEntity> _mapRepository;
        private readonly IRepository<RoomEntity> _roomRepository;

        public MapDomainService(IRepository<MapEntity> mapRepository, IRepository<RoomEntity> roomRepository, IMemoryCache cache, IMediatorHandler bus) : base(mapRepository, cache, bus)
        {
            _mapRepository = mapRepository;
            _roomRepository = roomRepository;
        }


        public async Task<int> GetRoomCount(int id)
        {
            var query = await _roomRepository.GetAll();

            return await query.CountAsync(x => x.MapId == id);
        }

    }
}
