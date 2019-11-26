using AutoMapper;
using Emprise.Application.Room.Models;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Player.Services;
using Emprise.Domain.Room.Models;
using Emprise.Domain.Room.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Emprise.Application.User.Services
{
    public class RoomAppService : IRoomAppService
    {
        private readonly IMediatorHandler _bus;
        private readonly IMapper _mapper;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly IRoomDomainService _roomDomainService;
        public RoomAppService(IMediatorHandler bus, IMapper mapper, IPlayerDomainService playerDomainService, IRoomDomainService roomDomainService)
        {
            _bus = bus;
            _mapper = mapper;
            _playerDomainService = playerDomainService;
            _roomDomainService = roomDomainService;
        }


        public async Task<CurrentRoomInfo> GetCurrent(int playerId)
        {
            var player = await _playerDomainService.Get(playerId);

            var room = await _roomDomainService.Get(player.RoomId);

            var ids = new List<int>() { room.West, room.East, room.South, room.North }.Where(x => x > 0);

            var rooms = await _roomDomainService.GetAll(x => ids.Contains(x.Id));

            rooms.Add(room);

            var roomInfos = _mapper.Map<List<RoomModel>>(rooms);

            var currentRoomInfo = new CurrentRoomInfo();
            currentRoomInfo.CurrentRoom = roomInfos.FirstOrDefault(x => x.Id == room.Id);
            currentRoomInfo.WestRoom = roomInfos.FirstOrDefault(x => x.Id == room.West);
            currentRoomInfo.EastRoom = roomInfos.FirstOrDefault(x => x.Id == room.East);
            currentRoomInfo.SouthRoom = roomInfos.FirstOrDefault(x => x.Id == room.South);
            currentRoomInfo.NorthRoom = roomInfos.FirstOrDefault(x => x.Id == room.North);
            return currentRoomInfo;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
