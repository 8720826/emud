using AutoMapper;
using Emprise.Application.Player.Dtos;
using Emprise.Application.Player.Models;
using Emprise.Domain.Core.Authorization;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Player.Commands;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.Player.Models;
using Emprise.Domain.Player.Services;
using Emprise.Infra.Extensions;
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
        private readonly IMediatorHandler _bus;
        private readonly IMapper _mapper;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly IAccountContext _account;

        public PlayerAppService(IMediatorHandler bus, IMapper mapper, IPlayerDomainService playerDomainService, ILogger<PlayerAppService> logger, IAccountContext account)
        {
            _bus = bus;
            _mapper = mapper;
            _playerDomainService = playerDomainService;
            _logger = logger;
            _account = account;
        }

        #region commond

        public async Task Create(int userId, PlayerCreateDto dto)
        {
            var commond = new CreateCommand(dto.Name, dto.Gender, userId, dto.Str, dto.Con, dto.Dex, dto.Int);
            await _bus.SendCommand(commond);
        }

        public async Task Delete(int id)
        {
            var commond = new DeleteCommand(id);
            await _bus.SendCommand(commond);
        }

        public async Task JoinGame(int userId, int playerId)
        {
            var commond = new JoinGameCommand(userId, playerId);
            await _bus.SendCommand(commond);
        }

        
        public async Task InitGame(int playerId)
        {
            _logger.LogDebug($"InitGame:{playerId}");
            var commond = new InitGameCommand(playerId);
            await _bus.SendCommand(commond).ConfigureAwait(false); ;
        }
        

        public async Task Move(int playerId, int roomId)
        {
            var commond = new MoveCommand(playerId, roomId);
            await _bus.SendCommand(commond);
        }

        public async Task Search(int playerId)
        {
            var commond = new SearchCommand(playerId);
            await _bus.SendCommand(commond);
        }

        public async Task Meditate(int playerId)
        {
            var commond = new MeditateCommand(playerId);
            await _bus.SendCommand(commond);
        }

        public async Task StopAction(int playerId)
        {
            var commond = new StopActionCommand(playerId);
            await _bus.SendCommand(commond);
        }

        public async Task Exert(int playerId)
        {
            var commond = new ExertCommand(playerId);
            await _bus.SendCommand(commond);
        }

        public async Task NpcAction(int playerId, int npcId, string action)
        {
            var commond = new NpcActionCommand(playerId, npcId, action);
            await _bus.SendCommand(commond);
        }


        #endregion

        public async Task<PlayerEntity> GetUserPlayer(int userId)
        {
            return await _playerDomainService.GetUserPlayer(userId);
        }

        public async Task<PlayerEntity> GetPlayer(int playerId)
        {
            return await _playerDomainService.Get(playerId);
        }


        public async Task<PlayerInfo> GetPlayerInfo(int playerId)
        {
            var playerInfo = new PlayerInfo()
            {
                Descriptions = new List<string>(), 
                Commands = new List<string>()
            };
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                return playerInfo;
            }
            playerInfo.Id = playerId;
            playerInfo.Name = player.Name;
            string genderStr = player.Gender.ToGender();


            //年龄
            playerInfo.Descriptions.Add($"{genderStr}{player.Age.ToAge()}");
          

            playerInfo.Descriptions.Add($"{genderStr}的武功看不出深浅。");
            playerInfo.Descriptions.Add($"{genderStr}看起来气血充盈，并没有受伤。");

            playerInfo.Commands.Add("切磋");
            playerInfo.Commands.Add("杀死");

            return playerInfo;
        }



        public async Task<MyInfo> GetMyInfo(int playerId)
        {
            var myInfo = new MyInfo()
            {

            };
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                return myInfo;
            }
            myInfo = _mapper.Map<MyInfo>(player);
            return myInfo;
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
