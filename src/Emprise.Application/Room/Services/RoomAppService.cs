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


        public async Task<RoomModel> GetCurrent(int playerId)
        {
            var player = await _playerDomainService.Get(playerId);

            var room = await _roomDomainService.Get(player.RoomId);

            var roomModel = _mapper.Map<RoomModel>(room);

            return roomModel;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
