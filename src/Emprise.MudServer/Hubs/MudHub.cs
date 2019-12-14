
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Emprise.Application.User.Services;
using Emprise.Domain.Core.Authorization;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Emprise.Domain.Core.Bus;
using Emprise.MudServer.Hubs.Models;
using Emprise.MudServer.Hubs.Actions;
using Emprise.Domain.Core.Notifications;
using MediatR;
using Emprise.Domain.Core.Models.Chat;
using Emprise.Domain.Player.Events;
using Emprise.Application.Player.Services;

namespace Emprise.MudServer.Hubs
{
    [Authorize]
    public class MudHub : BaseHub
    {
        private readonly INpcAppService _npcAppService;
        private readonly IDelayedQueue  _delayedQueue;
        private readonly IRecurringQueue  _recurringQueue;
        private readonly IMediatorHandler _bus;
        public MudHub(IAccountContext account, IMudOnlineProvider mudOnlineProvider, IOptions<AppConfig> appConfig, IPlayerAppService playerAppService,  ILogger<MudHub> logger, INpcAppService npcAppService, IDelayedQueue delayedQueue, IRecurringQueue recurringQueue, INotificationHandler<DomainNotification> notifications, IMediatorHandler bus) : base(account,notifications, mudOnlineProvider, appConfig, playerAppService, logger)
        {
            _npcAppService = npcAppService;
            _delayedQueue = delayedQueue;
            _recurringQueue = recurringQueue;
            _bus = bus;
        }

        public async Task Ping()
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                //更新玩家在线数据
                var model = await _mudOnlineProvider.GetPlayerOnline(playerId);
                if (model != null)
                {
                    await _mudOnlineProvider.SetPlayerOnline(new PlayerOnlineModel
                    {
                        IsOnline = true,
                        LastDate = DateTime.Now,
                        Level = model.Level,
                        PlayerName = model.PlayerName,
                        PlayerId = model.PlayerId,
                        RoomId = model.RoomId,
                        Gender = model.Gender,
                        Title = model.Title
                    });
                    return;
                }

                var player = await _playerAppService.GetPlayer(playerId);
                await _mudOnlineProvider.SetPlayerOnline(new PlayerOnlineModel
                {
                    IsOnline = true,
                    LastDate = DateTime.Now,
                    Level = player.Level,
                    PlayerName = player.Name,
                    PlayerId = player.Id,
                    RoomId = player.RoomId,
                    Gender = player.Gender,
                    Title = player.Title
                });
            });
        }

        public async Task Send(SendAction sendAction)
        {
            if (sendAction.Content.Length > 200)
            {
                await Clients.Client(Context.ConnectionId).ShowMessage(new PlayerMessage()
                {
                    Channel = "闲聊",
                    Content = "输入内容过多，不能超过100个字",
                    Sender = _account.PlayerName,
                    PlayerId = _account.PlayerId
                });
                return;
            }
            var result = await DoCommand(async ()=> {
                var receivedMessage = new PlayerMessage()
                {
                    Channel = "闲聊",
                    Content = WebUtility.HtmlEncode(sendAction.Content),
                    Sender = _account.PlayerName,
                    PlayerId = _account.PlayerId
                };

                await Clients.All.ShowMessage(receivedMessage);

            });

            if (result)
            {
                await _bus.RaiseEvent(new SendMessageEvent(_account.PlayerId, sendAction.Content)).ConfigureAwait(false);
                //await _recurringQueue.Publish(new MessageModel { Content = receivedMessage.Content, PlayerId = _account.PlayerId }, 5, 3);
            }
        }

        public async Task Move(MoveAction moveAction)
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;
                await _playerAppService.Move(playerId, moveAction.RoomId);
            });
        }

        public async Task ShowNpc(ShowNpcAction  showNpcAction)
        {
            var result = await DoCommand(async () => {
                var npc = await _npcAppService.GetNpc(showNpcAction.NpcId);
                await Clients.Client(Context.ConnectionId).ShowNpc(npc);
            });
        }

        public async Task ShowPlayer(ShowPlayerAction showPlayerAction)
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;
                if (playerId == showPlayerAction.PlayerId)
                {
                    var myInfo = await _playerAppService.GetMyInfo(playerId);
                    await Clients.Client(Context.ConnectionId).ShowMe(myInfo);
                    return;
                }

                var player = await _playerAppService.GetPlayerInfo(showPlayerAction.PlayerId);
                _logger.LogDebug($"player={showPlayerAction.PlayerId},{JsonConvert.SerializeObject(player)}");
                await Clients.Client(Context.ConnectionId).ShowPlayer(player);
            });
        }

        public async Task ShowMe()
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;
                var myInfo = await _playerAppService.GetMyInfo(playerId);
                await Clients.Client(Context.ConnectionId).ShowMe(myInfo);
            });
        }

        public async Task ShowMyStatus()
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;
                var myInfo = await _playerAppService.GetMyInfo(playerId);
                await Clients.Client(Context.ConnectionId).ShowMyStatus(myInfo);
            });
        }
        
        public async Task Search()
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;
                await _playerAppService.Search(playerId);
            });
        }

        /// <summary>
        /// 打坐
        /// </summary>
        /// <returns></returns>
        public async Task Meditate()
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;
                await _playerAppService.Meditate(playerId);
            });
        }


        public async Task StopAction()
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;
                await _playerAppService.StopAction(playerId);
            });
        }

        /// <summary>
        /// 疗伤
        /// </summary>
        /// <returns></returns>
        public async Task Exert()
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;
                await _playerAppService.Exert(playerId);
            });
        }


        public async Task NpcAction(NpcCommondAction moveAction)
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;
                await _playerAppService.NpcAction(playerId, moveAction.NpcId, moveAction.Action);
            });
        }
    }
}
