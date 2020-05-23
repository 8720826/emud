using Emprise.Domain.Common.Modes;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Room.Entity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Emprise.Domain.Room.Services
{
    public class RoomDomainService : IRoomDomainService
    {
        private readonly IRepository<RoomEntity> _roomRepository;
        private readonly IMemoryCache _cache;
        private readonly IMediatorHandler _bus;

        public RoomDomainService(IRepository<RoomEntity> roomRepository, IMemoryCache cache, IMediatorHandler bus)
        {
            _roomRepository = roomRepository;
            _cache = cache;
            _bus = bus;
        }


        public async Task<RoomEntity> Get(Expression<Func<RoomEntity, bool>> where)
        {
            return await _roomRepository.Get(where);
        }

        public async Task<List<RoomEntity>> GetAll(Expression<Func<RoomEntity, bool>> where)
        {
            var query = await _roomRepository.GetAll(where);

            return query.ToList();
        }

        public async Task<RoomEntity> Get(int id)
        {
            var key = string.Format(CacheKey.Room, id);
            return await _cache.GetOrCreateAsync(key, async p => {
                p.SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheKey.ExpireMinutes));
                return await _roomRepository.Get(id);
            });
        }

        public async Task Add(RoomEntity room)
        {
            await _roomRepository.Add(room);
        }

        public async Task Update(RoomEntity room)
        {
            await _roomRepository.Update(room);
            await _bus.RaiseEvent(new EntityUpdatedEvent<RoomEntity>(room)).ConfigureAwait(false);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
