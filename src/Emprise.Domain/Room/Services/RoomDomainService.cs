using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Services;
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
    public class RoomDomainService : BaseDomainService<RoomEntity>, IRoomDomainService
    {
        private readonly IRepository<RoomEntity> _roomRepository;
        private readonly IMemoryCache _cache;
        private readonly IMediatorHandler _bus;

        public RoomDomainService(IRepository<RoomEntity> roomRepository, IMemoryCache cache, IMediatorHandler bus) : base(roomRepository, cache, bus)
        {
            _roomRepository = roomRepository;
            _cache = cache;
            _bus = bus;
        }
    }
}
