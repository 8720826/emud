using Emprise.Application.Npc.Models;
using Emprise.Application.Player.Dtos;
using Emprise.Application.Player.Models;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.Player.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Application.Player.Services
{
    public interface IPlayerAppService : IBaseService
    {

        Task Create(int userId, PlayerCreateDto dto);

        Task Delete(int id);


        Task JoinGame(int userId, int playerId);

        Task InitGame(int playerId);

        Task Move(int playerId, int roomId);

        Task Search(int playerId);

        Task Meditate(int playerId);

        Task StopAction(int playerId);

        Task Exert(int playerId);

        Task NpcAction(int playerId, int npcId, NpcAction action);

        Task TakeQuest(int playerId, int questId);

        Task CompleteQuest(int playerId, int questId);

        Task<PlayerEntity> GetUserPlayer(int userId);

        Task<PlayerEntity> GetPlayer(int playerId);

        Task<PlayerInfo> GetPlayerInfo(int playerId);

        Task<MyInfo> GetMyInfo(int playerId);


    }
}
