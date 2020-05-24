using AutoMapper;
using Emprise.Application.Npc.Models;
using Emprise.Application.Player.Dtos;
using Emprise.Application.Player.Models;
using Emprise.Domain.Core.Authorization;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Player.Commands;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.Player.Models;
using Emprise.Domain.Player.Services;
using Emprise.Domain.Ware.Models;
using Emprise.Domain.Ware.Services;
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
        private readonly IPlayerWareDomainService _playerWareDomainService;
        private readonly IWareDomainService _wareDomainService;
        private readonly IAccountContext _account;

        public PlayerAppService(IMediatorHandler bus, 
            IMapper mapper, 
            IPlayerDomainService playerDomainService, 
            ILogger<PlayerAppService> logger,
            IPlayerWareDomainService playerWareDomainService,
            IWareDomainService wareDomainService,
            IAccountContext account)
        {
            _bus = bus;
            _mapper = mapper;
            _playerDomainService = playerDomainService;
            _logger = logger;
            _account = account;
            _playerWareDomainService = playerWareDomainService;
            _wareDomainService = wareDomainService;
        }

        #region command

        public async Task Create(int userId, PlayerCreateDto dto)
        {
            var command = new CreateCommand(dto.Name, dto.Gender, userId, dto.Str, dto.Con, dto.Dex, dto.Int);
            await _bus.SendCommand(command);
        }

        public async Task Delete(int id)
        {
            var command = new DeleteCommand(id);
            await _bus.SendCommand(command);
        }

        public async Task JoinGame(int userId, int playerId)
        {
            var command = new JoinGameCommand(userId, playerId);
            await _bus.SendCommand(command);
        }


        public async Task InitGame(int playerId)
        {
            _logger.LogDebug($"InitGame:{playerId}");
            var command = new InitGameCommand(playerId);
            await _bus.SendCommand(command).ConfigureAwait(false); ;
        }


        public async Task Move(int playerId, int roomId)
        {
            var command = new MoveCommand(playerId, roomId);
            await _bus.SendCommand(command);
        }

        public async Task Search(int playerId)
        {
            var command = new SearchCommand(playerId);
            await _bus.SendCommand(command);
        }

        public async Task Meditate(int playerId)
        {
            var command = new MeditateCommand(playerId);
            await _bus.SendCommand(command);
        }

        public async Task StopAction(int playerId)
        {
            var command = new StopActionCommand(playerId);
            await _bus.SendCommand(command);
        }

        public async Task Exert(int playerId)
        {
            var command = new ExertCommand(playerId);
            await _bus.SendCommand(command);
        }

        public async Task NpcAction(int playerId, int npcId, NpcAction action)
        {
            var command = new NpcActionCommand(playerId, npcId, action.ScriptId, action.CommandId, action.Name, action.Message);
            await _bus.SendCommand(command);
        }

        public async Task TakeQuest(int playerId, int questId)
        {
            var command = new QuestCommand(playerId, questId);
            await _bus.SendCommand(command);
        }

        public async Task CompleteQuest(int playerId, int questId)
        {
            var command = new CompleteQuestCommand(playerId, questId);
            await _bus.SendCommand(command);
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

        public async Task<MyPack> GetMyPack(int playerId)
        {
            var myPack = new MyPack()
            {
                Money = "", 
                Wares = new List<WareModel>()

            };
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                return myPack;
            }

            myPack.Money = player.Money.ToMoney();

            var playerWares = await _playerWareDomainService.GetAll(player.Id);
            if (playerWares == null || playerWares.Count == 0)
            {
                return myPack;
            }

            var ids = playerWares.Select(x => x.WareId);

            var wares = await _wareDomainService.GetAll(x => ids.Contains(x.Id));
            foreach (var playerWare in playerWares)
            {
                var ware = wares.FirstOrDefault(x=>x.Id== playerWare.WareId);
                if (ware != null)
                {
                    var wareModel = _mapper.Map<WareModel>(ware);
                    wareModel.Number = playerWare.Number;
                    wareModel.Status = playerWare.Status;
                    myPack.Wares.Add(wareModel);
                }

            }

            return myPack;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
