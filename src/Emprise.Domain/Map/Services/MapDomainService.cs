using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Models;
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
    public class MapDomainService : IMapDomainService
    {
        private readonly IRepository<MapEntity> _mapRepository;
        private readonly IRepository<RoomEntity> _roomRepository;
        private readonly IMemoryCache _cache;
        private readonly IMediatorHandler _bus;

        public MapDomainService(IRepository<MapEntity> mapRepository, IRepository<RoomEntity> roomRepository, IMemoryCache cache, IMediatorHandler bus)
        {
            _mapRepository = mapRepository;
            _mapRepository = mapRepository;
            _cache = cache;
            _bus = bus;
        }


        public async Task<MapEntity> Get(Expression<Func<MapEntity, bool>> where)
        {
            return await _mapRepository.Get(where);
        }


        public async Task<MapEntity> Get(int id)
        {
            var key = string.Format(CacheKey.Map, id);
            return await _cache.GetOrCreateAsync(key, async p => {
                p.SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheKey.ExpireMinutes));
                return await _mapRepository.Get(id);
            });
        }

        public async Task Add(MapEntity entity)
        {
            await _mapRepository.Add(entity);
        }

        public async Task<int> GetRoomCount(int id)
        {
            var query = await _roomRepository.GetAll();

            return await query.CountAsync(x => x.MapId == id);
        }

        public async Task<IQueryable<MapEntity>> GetAll()
        {
            return await _mapRepository.GetAll();
        }

        public async Task Update(MapEntity entity)
        {
            await _mapRepository.Update(entity);
            await _bus.RaiseEvent(new EntityUpdatedEvent<MapEntity>(entity)).ConfigureAwait(false);
        }

        public async Task Delete(MapEntity entity)
        {
            await _mapRepository.Remove(entity);
            await _bus.RaiseEvent(new EntityDeletedEvent<MapEntity>(entity)).ConfigureAwait(false);
        }
    }
}
