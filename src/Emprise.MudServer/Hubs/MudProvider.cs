using Emprise.Domain.Core.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Emprise.MudServer.Hubs.Models;
using Emprise.Domain.Core.Enum;

namespace Emprise.MudServer.Hubs
{
    public class MudProvider: IMudProvider
    {
        private readonly IMudOnlineProvider _chatOnlineProvider;
        private readonly IHubContext<MudHub> _context;
        public MudProvider(IHubContext<MudHub> context, IMudOnlineProvider chatOnlineProvider)
        {
            _context = context;
            _chatOnlineProvider = chatOnlineProvider;
        }

        /// <summary>
        /// 系统发消息给所有人
        /// </summary>
        /// <param name="content"></param>
        /// <param name="channel"></param>
        public async Task ShowAllMessage(string content, MessageTypeEnum type = MessageTypeEnum.提示)
        {
            await Send(new GroupHubData
            {
                Action = ClientMethod.ShowMessage,
                GroupId = "",//为空表示发送给所有人
                Data = new SystemMessage()
                {
                    Type = type,
                    Content = content
                }
            });
        }

        
        /// <summary>
        /// 系统发消息给所有其他人
        /// nickName与connectionId有一个有值
        /// </summary>
        /// <param name="nickName"></param>
        /// <param name="connectionId"></param>
        /// <param name="content"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public async Task ShowOthersMessage(int playerId, string content, MessageTypeEnum type = MessageTypeEnum.提示)
        {
            await Send(new GroupExceptHubData
            {
                Action = ClientMethod.ShowMessage,
                GroupId = "",//为空表示发送给所有人
                PlayerId = playerId,
                Data = new SystemMessage()
                {
                    Type = type,
                    Content = content
                }
            });
        }
        

        /// <summary>
        /// 系统发消息给该场景的所有人
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="content"></param>
        /// <param name="channel"></param>
        public async Task ShowRoomMessage(int roomId, string content, MessageTypeEnum type = MessageTypeEnum.提示)
        {
            await Send(new GroupHubData
            {
                Action = ClientMethod.ShowMessage,
                GroupId = $"room_{roomId}",
                Data = new SystemMessage()
                {
                    Type = type,
                    Content = content
                }
            });
        }

        /// <summary>
        /// 系统发送消息给该场景的其他人
        /// nickName与connectionId有一个有值
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="nickName"></param>
        /// <param name="connectionId"></param>
        /// <param name="content"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public async Task ShowRoomOthersMessage(int roomId, int playerId, string content, MessageTypeEnum type = MessageTypeEnum.提示)
        {
            await Send(new GroupExceptHubData
            {
                Action = ClientMethod.ShowMessage,
                GroupId = $"room_{roomId}",
                PlayerId = playerId,
                Data = new SystemMessage()
                {
                    Type = type,
                    Content = content
                }
            });
        }

        /// <summary>
        /// 系统发送消息给个人
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="content"></param>
        /// <param name="channel"></param>
        public async Task ShowMessage(int playerId, string content, MessageTypeEnum type = MessageTypeEnum.提示)
        {
            if (string.IsNullOrEmpty(content))
            {
                return;
            }
            await Send(new PlayerHubData
            {
                Action = ClientMethod.ShowMessage,
                PlayerId = playerId,
                Data = new SystemMessage()
                {
                    Type = type,
                    Content = content
                }
            });
        }

        /// <summary>
        /// 发送Hub消息到客户端
        /// </summary>
        /// <param name="message"></param>
        public async Task Send(HubData message)
        {
            if (message is PlayerHubData)
            {
                var userHubMessage = message as PlayerHubData;
                await _context.Clients.User(userHubMessage.PlayerId.ToString()).SendAsync(userHubMessage.Action, message.Data);
            }
            else if (message is GroupHubData)
            {
                var groupHubMessage = message as GroupHubData;
                if (string.IsNullOrEmpty(groupHubMessage.GroupId)) 
                {
                    await _context.Clients.All.SendAsync(groupHubMessage.Action, message.Data);
                } 
                else
                {
                    await _context.Clients.Group(groupHubMessage.GroupId).SendAsync(groupHubMessage.Action, message.Data);
                }
              
            }
            else if (message is GroupExceptHubData)
            {
                var groupExceptHubMessage = message as GroupExceptHubData;
                var playerId = groupExceptHubMessage.PlayerId;
                var connectionId = await _chatOnlineProvider.GetConnectionId(playerId);

                if (string.IsNullOrEmpty(groupExceptHubMessage.GroupId))
                {
                    await _context.Clients.AllExcept(connectionId).SendAsync(groupExceptHubMessage.Action, message.Data);
                }
                else
                {
                    await _context.Clients.GroupExcept(groupExceptHubMessage.GroupId, connectionId).SendAsync(groupExceptHubMessage.Action, message.Data);
                }
            }
        }

        public async Task UpdateRoomPlayerList(int roomId, object obj)
        {
            await _context.Clients.Group($"room_{roomId}").SendAsync(ClientMethod.UpdatePlayerList, obj);
        }


        public async Task UpdateRoomNpcList(int roomId, object obj)
        {
            await _context.Clients.Group($"room_{roomId}").SendAsync(ClientMethod.UpdateNpcList, obj);
        }

        public async Task UpdatePlayerRoomNpcList(int playerId, object obj)
        {
            await _context.Clients.User(playerId.ToString()).SendAsync(ClientMethod.UpdateNpcList, obj);
        }

        public async Task UpdatePlayerRoomPlayerList(int playerId, object obj)
        {
            await _context.Clients.User(playerId.ToString()).SendAsync(ClientMethod.UpdatePlayerList, obj);
        }

        public async Task RemoveFromGroup(int playerId, string groupName)
        {
            var connectionId = await _chatOnlineProvider.GetConnectionId(playerId);
            if (string.IsNullOrEmpty(connectionId))
            {
                return;
            }
            await _context.Groups.RemoveFromGroupAsync(connectionId, groupName);
        }

        public async Task AddToGroup(int playerId, string groupName)
        {
            var connectionId = await _chatOnlineProvider.GetConnectionId(playerId);
            if (string.IsNullOrEmpty(connectionId))
            {
                return;
            }
            await _context.Groups.AddToGroupAsync(connectionId, groupName);
        }

        public async Task RemoveFromRoom(int playerId, int roomId)
        {
            var connectionId = await _chatOnlineProvider.GetConnectionId(playerId);
            if (string.IsNullOrEmpty(connectionId))
            {
                return;
            }
            await _context.Groups.RemoveFromGroupAsync(connectionId, $"room_{roomId}");
        }

        public async Task AddToRoom(int playerId, int roomId)
        {
            var connectionId = await _chatOnlineProvider.GetConnectionId(playerId);
            if (string.IsNullOrEmpty(connectionId))
            {
                return;
            }
            await _context.Groups.AddToGroupAsync(connectionId, $"room_{roomId}");
        }


        public async Task Move(int playerId, object obj)
        {
            await _context.Clients.User(playerId.ToString()).SendAsync(ClientMethod.Move, obj);
        }


        public async Task ShowRevive(int playerId)
        {
            await _context.Clients.User(playerId.ToString()).SendAsync("showRevive");
        }

        public async Task RefreshKillInfo(int playerId, object obj)
        {
            await _context.Clients.User(playerId.ToString()).SendAsync("refreshKillInfo", obj);
        }

        public async Task UpdatePlayerInfo(int playerId, object obj)
        {
            await _context.Clients.User(playerId.ToString()).SendAsync("UpdatePlayerInfo", obj);
        }

        public async Task UpdatePlayerStatus(int playerId, object obj)
        {
            await _context.Clients.User(playerId.ToString()).SendAsync("UpdatePlayerStatus", obj);
        }

    }
}
