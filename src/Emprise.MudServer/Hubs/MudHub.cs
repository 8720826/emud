
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Emprise.Domain.Core.Authorization;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Emprise.Domain.Core.Bus;
using Emprise.MudServer.Hubs.Models;
using Emprise.MudServer.Hubs.Actions;
using Emprise.Domain.Core.Notifications;
using MediatR;
using Emprise.MudServer.Commands;
using Emprise.MudServer.Commands.EmailCommands;
using Emprise.Infra.Authorization;
using Emprise.MudServer.Commands.WareCommands;
using Emprise.MudServer.Commands.QuestCommands;
using Emprise.MudServer.Commands.SkillCommands;
using Emprise.MudServer.Commands.RelationCommonds;
using Emprise.Domain.Core.Enums;
using Emprise.MudServer.Commands.NpcActionCommands;
using Emprise.MudServer.Commands.NpcCommands;

namespace Emprise.MudServer.Hubs
{
    [UserAuthorize]
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

        public async Task ShowMyWeapon()
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new ShowMyWeaponCommand(playerId);
                await _bus.SendCommand(command);
            });
        }
        

        public async Task ShowMySkill()
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new ShowMySkillCommand(playerId);
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


        public async Task ShowEmailDetail(ShowEmailDetailAction  showEmailDetailAction)
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new ShowEmailDetailCommand(playerId, showEmailDetailAction.PlayerEmailId);
                await _bus.SendCommand(command);
            });
        }

        public async Task DeleteEmail(DeleteEmailAction deleteEmailAction)
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new DeleteEmailCommand(playerId, deleteEmailAction.PlayerEmailId);
                await _bus.SendCommand(command);
            });
        }

        public async Task ReadEmail(ReadEmailAction showEmailAction)
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new ReadEmailCommand(playerId, showEmailAction.PlayerEmailId);
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

        public async Task Fish()
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new FishCommand(playerId);
                await _bus.SendCommand(command);
            });
        }

        public async Task Dig()
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new DigCommand(playerId);
                await _bus.SendCommand(command);
            });
        }

        public async Task Collect()
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new CollectCommand(playerId);
                await _bus.SendCommand(command);
            });
        }

        public async Task Cut()
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new CutCommand(playerId);
                await _bus.SendCommand(command);
            });
        }

        public async Task Hunt()
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new HuntCommand(playerId);
                await _bus.SendCommand(command);
            });
        }


        public async Task NpcAction(NpcCommandAction commandAction)
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var npcId = commandAction.NpcId;
                var commandId = commandAction.Action.CommandId;
                var commandName = commandAction.Action.Name;
                var input = commandAction.Action.Message;
                var scriptId = commandAction.Action.ScriptId;



                if (scriptId > 0)
                {
                    var command = new NpcScriptCommand(playerId, commandAction.NpcId, commandAction.Action.ScriptId, commandAction.Action.CommandId,  commandAction.Action.Message);
                    await _bus.SendCommand(command);
                }
                else
                {
                    NpcActionEnum actionEnum;
                    if (Enum.TryParse(commandName, out actionEnum))
                    {
                        switch (actionEnum)
                        {
                            case NpcActionEnum.闲聊:

                                await _bus.SendCommand(new ChatWithNpcCommand(playerId, commandAction.NpcId));

                                break;

                            case NpcActionEnum.切磋:

                                await _bus.SendCommand(new FightWithNpcCommand(playerId, commandAction.NpcId));

                                break;

                            case NpcActionEnum.杀死:

                                await _bus.SendCommand(new KillNpcCommand(playerId, commandAction.NpcId));

                                break;

                            case NpcActionEnum.给予:

                                await _bus.SendCommand(new GiveToNpcCommand(playerId, commandAction.NpcId));

                                break;

                            case NpcActionEnum.拜师:

                                await _bus.SendCommand(new ApprenticeToNpcCommand(playerId, commandAction.NpcId));

                                break;

                            case NpcActionEnum.出师:

                                await _bus.SendCommand(new FinishApprenticeToNpcCommand(playerId, commandAction.NpcId));

                                break;

                            case NpcActionEnum.查看武功:

                                await _bus.SendCommand(new ShowNpcSkillCommand(playerId, commandAction.NpcId));

                                break;
                                
                        }
                    }
                }


            });
        }


        public async Task TakeQuest(QuestDetailAction questAction)
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new QuestCommand(playerId, questAction.QuestId);
                await _bus.SendCommand(command);
            });
        }

        public async Task CompleteQuest(QuestDetailAction questAction)
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

                var command = new LoadWareCommand(playerId, wareAction.MyWareId);
                await _bus.SendCommand(command);
            });
        }

        public async Task UnLoad(WareAction wareAction)
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;
 
                var command = new UnLoadWareCommand(playerId, wareAction.MyWareId);
                await _bus.SendCommand(command);
            });
        }

        public async Task Learn(WareAction wareAction)
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new LearnWareCommand(playerId, wareAction.MyWareId);
                await _bus.SendCommand(command);
            });
        }

        public async Task ShowWare(WareAction wareAction)
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new ShowWareCommand(playerId, wareAction.MyWareId);
                await _bus.SendCommand(command);
            });
        }

        public async Task Drop(WareAction wareAction)
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new DropWareCommand(playerId, wareAction.MyWareId);
                await _bus.SendCommand(command);
            });
        }

        public async Task ShowMyQuest(QuestAction questAction)
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new ShowMyQuestCommand(playerId, questAction.Type);
                await _bus.SendCommand(command);
            });
        }

        public async Task ShowMyHistoryQuest()
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new ShowMyHistoryQuestCommand(playerId);
                await _bus.SendCommand(command);
            });
        }

        public async Task ShowQuestDetail(QuestDetailAction questAction)
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new ShowQuestDetailCommand(playerId, questAction.QuestId);
                await _bus.SendCommand(command);
            });
        }


        public async Task ShowSkillDetail(SkillDetailAction questAction)
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new ShowSkillDetailCommand(playerId, questAction.ObjectSkillId, questAction.Type);
                await _bus.SendCommand(command);
            });
        }



        public async Task LearnSkill(SkillDetailAction questAction)
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new LearnSkillCommand(playerId, questAction.ObjectSkillId, questAction.Type);
                await _bus.SendCommand(command);
            });
        }

        public async Task PlayerAction(PlayerCommandAction commandAction)
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;
                var targetId = commandAction.TargetId;
                var commandName = commandAction.CommandName;

                PlayerActionEnum actionEnum;
                if (!Enum.TryParse(commandName, out actionEnum))
                {
                    return;
                }

                switch (actionEnum)
                {
                    case PlayerActionEnum.添加好友:
                        await _bus.SendCommand(new FriendToCommand(playerId, commandAction.TargetId));
                        break;

                    case PlayerActionEnum.割袍断义:
                        await _bus.SendCommand(new UnFriendToCommand(playerId, commandAction.TargetId));
                        break;

                    case PlayerActionEnum.查看武功:
                        await _bus.SendCommand(new ShowFriendSkillCommand(playerId, commandAction.TargetId));
                        break;
                }


            });
        }

        public async Task ShowFriend()
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new ShowFriendCommand(playerId);
                await _bus.SendCommand(command);
            });
        }

        public async Task AgreeFriend(RelationAction relationAction)
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new AgreeFriendCommand(playerId, relationAction.RelationId);
                await _bus.SendCommand(command);
            });
        }

        public async Task RefuseFriend(RelationAction relationAction)
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new RefuseFriendCommand(playerId, relationAction.RelationId);
                await _bus.SendCommand(command);
            });
        }


        public async Task ShowShop()
        {
            var result = await DoCommand(async () => {
                var playerId = _account.PlayerId;

                var command = new ShowShopCommand(playerId);
                await _bus.SendCommand(command);
            });
        }
        
    }
}
