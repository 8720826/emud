
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
using Emprise.MudServer.Commands;
using Emprise.MudServer.Events;
using Emprise.MudServer.Commands.EmailCommands;

namespace Emprise.MudServer.Hubs
{
    [Authorize]
    public class MudHub : BaseHub
    {
        private readonly IDelayedQueue  _delayedQueue;
        private readonly IRecurringQueue  _recurringQueue;
        private readonly IMediatorHandler _bus;
        public MudHub(IAccountContext account, IMudOnlineProvider mudOnlineProvider, IOptionsMonitor<AppConfig> appConfig,  ILogger<MudHub> logger,  IDelayedQueue delayedQueue, IRecurringQueue recurringQueue, INotificationHandler<DomainNotification> notifications, IMediatorHandler bus) : base(account,notifications, mudOnlineProvider, appConfig,  logger, bus)
        {
            _delayedQueue = delayedQueue;
            _recurringQueue = recurringQueue;
            _bus = bus;
        }

        public async Task Ping()
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new PingCommand(playerId);
                await _bus.SendCommand(command);


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
                return ;
            }

            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new SendMessageCommand(playerId, sendAction.Channel, sendAction.Content);
                await _bus.SendCommand(command);
            });
        }

        public async Task Move(MoveAction moveAction)
        {

            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new MoveCommand(playerId, moveAction.RoomId);
                await _bus.SendCommand(command);
            });
        }

        public async Task ShowNpc(ShowNpcAction  showNpcAction)
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new ShowNpcCommand(playerId, showNpcAction.NpcId);
                await _bus.SendCommand(command);
            });
        }

        public async Task ShowPlayer(ShowPlayerAction showPlayerAction)
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;
                if (playerId == showPlayerAction.PlayerId)
                {
                    var command = new ShowMeCommand(playerId);
                    await _bus.SendCommand(command);
                    return;
                }

                var commandShowPlayer = new ShowPlayerCommand(playerId, showPlayerAction.PlayerId);
                await _bus.SendCommand(commandShowPlayer);
            });
        }

        public async Task ShowMe()
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new ShowMeCommand(playerId);
                await _bus.SendCommand(command);
            });
        }

        public async Task ShowMyStatus()
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new ShowMyStatusCommand(playerId);
                await _bus.SendCommand(command);
            });
        }

        public async Task ShowMyPack()
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new ShowMyPackCommand(playerId);
                await _bus.SendCommand(command);
            });
        }

        public async Task ShowEmail(ShowEmailAction showEmailAction)
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new ShowEmailCommand(playerId, showEmailAction.PageIndex);
                await _bus.SendCommand(command);
            });
        }
        

        public async Task Search()
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new SearchCommand(playerId);
                await _bus.SendCommand(command);
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

                var command = new MeditateCommand(playerId);
                await _bus.SendCommand(command);
            });
        }


        public async Task StopAction()
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new StopActionCommand(playerId);
                await _bus.SendCommand(command);
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

                var command = new ExertCommand(playerId);
                await _bus.SendCommand(command);
            });
        }


        public async Task NpcAction(NpcCommandAction commandAction)
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new NpcActionCommand(playerId, commandAction.NpcId, commandAction.Action.ScriptId, commandAction.Action.CommandId, commandAction.Action.Name, commandAction.Action.Message);
                await _bus.SendCommand(command);
            });
        }


        public async Task TakeQuest(QuestAction questAction)
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new QuestCommand(playerId, questAction.QuestId);
                await _bus.SendCommand(command);
            });
        }

        public async Task CompleteQuest(QuestAction questAction)
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new CompleteQuestCommand(playerId, questAction.QuestId);
                await _bus.SendCommand(command);
            });
        }

        public async Task Load(WareAction wareAction)
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new LoadWareCommand(playerId, wareAction.WareId);
                await _bus.SendCommand(command);
            });
        }

        public async Task UnLoad(WareAction wareAction)
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;
 
                var command = new UnLoadWareCommand(playerId, wareAction.WareId);
                await _bus.SendCommand(command);
            });
        }


        
    }
}
