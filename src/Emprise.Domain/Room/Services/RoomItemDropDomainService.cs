using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Models;
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
    public class RoomItemDropDomainService : IRoomItemDropDomainService
    {
        private readonly IRepository<RoomItemDropEntity> _roomRepository;
        private readonly IMemoryCache _cache;
        private readonly IMediatorHandler _bus;

        public RoomItemDropDomainService(IRepository<RoomItemDropEntity> roomRepository, IMemoryCache cache, IMediatorHandler bus)
        {
            _roomRepository = roomRepository;
            _cache = cache;
            _bus = bus;
        }


        public async Task<RoomItemDropEntity> Get(Expression<Func<RoomItemDropEntity, bool>> where)
        {
            return await _roomRepository.Get(where);
        }

        public async Task<IQueryable<RoomItemDropEntity>> GetAll()
        {
            return await _roomRepository.GetAll();

        }

        public async Task<RoomItemDropEntity> Get(int id)
        {
            return await _roomRepository.Get(id);
        }

        public async Task Add(RoomItemDropEntity room)
        {
            await _roomRepository.Add(room);
        }

        public async Task Update(RoomItemDropEntity room)
        {
            await _roomRepository.Update(room);
            await _bus.RaiseEvent(new EntityUpdatedEvent<RoomItemDropEntity>(room)).ConfigureAwait(false);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
