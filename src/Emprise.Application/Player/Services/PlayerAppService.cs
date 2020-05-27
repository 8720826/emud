using AutoMapper;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.Player.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Emprise.Application.Player.Services
{
    public class PlayerAppService : IPlayerAppService
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IPlayerDomainService _playerDomainService;

        public PlayerAppService(
            IMapper mapper, 
            IPlayerDomainService playerDomainService, 
            ILogger<PlayerAppService> logger)
        {
            _mapper = mapper;
            _playerDomainService = playerDomainService;
            _logger = logger;
        }



        public async Task<PlayerEntity> GetUserPlayer(int userId)
        {
            return await _playerDomainService.GetUserPlayer(userId);
        }

        public async Task<PlayerEntity> GetPlayer(int playerId)
        {
            return await _playerDomainService.Get(playerId);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
