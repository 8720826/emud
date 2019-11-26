using Emprise.Domain.Core.Enum;
using Emprise.Domain.Core.Interfaces.Ioc;
using Emprise.Domain.Core.Models.Chat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Core.Interfaces
{
    public interface IMudProvider: IScoped
    {
        Task ShowAllMessage(string content, MessageTypeEnum type = MessageTypeEnum.提示);

        Task ShowOthersMessage(int playerId, string content, MessageTypeEnum type = MessageTypeEnum.提示);

        Task ShowRoomMessage(int roomId, string content, MessageTypeEnum type = MessageTypeEnum.提示);

        Task ShowRoomOthersMessage(int roomId, int playerId, string content, MessageTypeEnum type = MessageTypeEnum.提示);

        Task ShowMessage(int playerId, string content, MessageTypeEnum type = MessageTypeEnum.提示);

        Task UpdateRoomPlayerList(int roomId, object obj);

        Task UpdatePlayerRoomNpcList(int playerId, object obj);


        Task UpdatePlayerRoomPlayerList(int playerId, object obj);

        Task UpdateRoomNpcList(int roomId, object obj);

        Task RemoveFromGroup(int playerId, string groupName);

        Task AddToGroup(int playerId, string groupName);

        Task RemoveFromRoom(int playerId, int roomId);

        Task AddToRoom(int playerId, int roomId);


        Task Move(int playerId, object obj);

        Task ShowRevive(int playerId);

        Task RefreshKillInfo(int playerId, object obj);

        Task UpdatePlayerInfo(int playerId, object obj);

        Task UpdatePlayerStatus(int playerId, object obj);

    }
}
