using AutoMapper;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.EventHandlers;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Models.Chat;
using Emprise.Domain.Log.Entity;
using Emprise.Domain.Log.Services;
using Emprise.Domain.Npc.Services;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.Player.Models;
using Emprise.Domain.Player.Services;
using Emprise.Domain.Quest.Entity;
using Emprise.Domain.Quest.Models;
using Emprise.Domain.Quest.Services;
using Emprise.Domain.Room.Models;
using Emprise.Domain.Room.Services;
using Emprise.MudServer.Events;
using Emprise.MudServer.Queues;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emprise.MudServer.EventHandlers
{
    public class PlayerEventHandler : MudEventHandler,
        INotificationHandler<EntityUpdatedEvent<PlayerEntity>>,
        INotificationHandler<EntityInsertedEvent<PlayerEntity>>,
        INotificationHandler<EntityDeletedEvent<PlayerEntity>>,
        INotificationHandler<MovedEvent>,
        INotificationHandler<PlayerInRoomEvent>,
        INotificationHandler<InitGameEvent>,
        INotificationHandler<PlayerStatusChangedEvent>,
        INotificationHandler<SendMessageEvent>,
        INotificationHandler<CompleteQuestEvent>,
        INotificationHandler<QuestTriggerEvent>

        


    {
        public const string Player = "Player_{0}";

        private readonly IRoomDomainService _roomDomainService;
        private readonly INpcDomainService _npcDomainService;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly IPlayerQuestDomainService _playerQuestDomainService;
        private readonly IMudProvider _mudProvider;
        private readonly IMudOnlineProvider _chatOnlineProvider;
        private readonly IMapper  _mapper;
        private readonly AppConfig _appConfig;
        private readonly IChatLogDomainService _chatLogDomainService;
        private readonly IQuestDomainService _questDomainService;
        private readonly ILogger<PlayerEventHandler> _logger;
        private readonly IQueueHandler _queueHandler;

        public PlayerEventHandler(IRoomDomainService roomDomainService, 
            INpcDomainService npcDomainService, 
            IPlayerDomainService playerDomainService, 
            IMudProvider mudProvider, 
            IMudOnlineProvider chatOnlineProvider, 
            IMapper mapper, 
            IOptionsMonitor<AppConfig> appConfig, 
            IChatLogDomainService chatLogDomainService,
            IQuestDomainService questDomainService,
            ILogger<PlayerEventHandler> logger,
            IPlayerQuestDomainService playerQuestDomainService,
            IQueueHandler queueHandler,
            IUnitOfWork uow) 
            : base(uow, mudProvider)
        {
            _roomDomainService = roomDomainService;
            _npcDomainService = npcDomainService;
            _playerDomainService = playerDomainService;
            _mudProvider = mudProvider;
            _chatOnlineProvider = chatOnlineProvider;
            _mapper = mapper;
            _appConfig = appConfig.CurrentValue;
            _chatLogDomainService = chatLogDomainService;
            _questDomainService = questDomainService;
            _logger = logger;
            _playerQuestDomainService = playerQuestDomainService;
            _queueHandler = queueHandler;
        }

        public async Task Handle(EntityUpdatedEvent<PlayerEntity> message, CancellationToken cancellationToken)
        {

        }

        public async Task Handle(EntityInsertedEvent<PlayerEntity> message, CancellationToken cancellationToken)
        {
            await Task.Run(() => {
               
            });
        }

        public async Task Handle(EntityDeletedEvent<PlayerEntity> message, CancellationToken cancellationToken)
        {

        }
        
        public async Task Handle(MovedEvent message, CancellationToken cancellationToken)
        {
            var player = message.Player;
            var roomIn = message.RoomIn;
            var roomOut = message.RoomOut;

            //更新玩家在线数据
            await _chatOnlineProvider.SetPlayerOnline(new PlayerOnlineModel
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

            //更新玩家聊天房间
            await _mudProvider.RemoveFromRoom(player.Id, roomOut.Id);
            await _mudProvider.AddToRoom(player.Id, roomIn.Id);

            //更新新房间玩家列表
            var playersIn = await _chatOnlineProvider.GetPlayerList(roomIn.Id);
            await _mudProvider.UpdateRoomPlayerList(roomIn.Id, playersIn);

            //更新旧房间玩家列表
            var playersOut = await _chatOnlineProvider.GetPlayerList(roomOut.Id);
            await _mudProvider.UpdateRoomPlayerList(roomOut.Id, playersOut);


            var roomModel = _mapper.Map<RoomModel>(roomIn);

            await _mudProvider.Move(player.Id, roomModel);

            //更新当前玩家显示的npc列表
            var nps = await _npcDomainService.Query(x => x.RoomId == roomIn.Id);
            await _mudProvider.UpdatePlayerRoomNpcList(player.Id, nps);


            //输出移动信息
            await _mudProvider.ShowMessage(player.Id, $"你来到了{roomIn.Name}。");
            await _mudProvider.ShowRoomOthersMessage(roomIn.Id, player.Id, $"[{player.Name}] 从{roomOut.Name}走了过来。");
            await _mudProvider.ShowRoomOthersMessage(roomOut.Id, player.Id, $"[{player.Name}] 往{roomIn.Name}离开。");



        }
        

        public async Task Handle(PlayerInRoomEvent message, CancellationToken cancellationToken)
        {
            var player = message.Player;
            var room = message.Room;

            //var model = await _chatOnlineProvider.GetPlayerOnline(player.Id);
            //var isOnline = model != null;


            await _chatOnlineProvider.SetPlayerOnline(new PlayerOnlineModel
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

            var roomModel = _mapper.Map<RoomModel>(room);

            await _mudProvider.Move(player.Id, roomModel);


            var nps = await _npcDomainService.Query(x => x.RoomId == room.Id);
            await _mudProvider.UpdatePlayerRoomNpcList(player.Id, nps);


            await _mudProvider.AddToRoom(player.Id, room.Id);

            await _mudProvider.ShowMessage(player.Id, $"你来到了{room.Name}。");


            var players = await _chatOnlineProvider.GetPlayerList(room.Id);
            await _mudProvider.UpdateRoomPlayerList(room.Id, players);

            var myInfo = _mapper.Map<MyInfo>(player);
            await _mudProvider.UpdatePlayerInfo(player.Id, myInfo);
        }

        public async Task Handle(InitGameEvent message, CancellationToken cancellationToken)
        {
            var player = message.Player;

            var model = await _chatOnlineProvider.GetPlayerOnline(player.Id);
            var isOnline = model != null;

            if (isOnline)
            {
                await _mudProvider.ShowMessage(player.Id, $"<font color=#FF5555>你重新连接到【{_appConfig.Site.Name}】。</font>");
            }
            else
            {
                await _mudProvider.ShowMessage(player.Id, _appConfig.Site.WelcomeWords.Replace("{name}", player.Name));
            }

            //await CheckPlayerMainQuest(player);

            await _queueHandler.SendQueueMessage(new CheckPlayerMainQuestQueue(player.Id));

        }

        public async Task Handle(PlayerStatusChangedEvent message, CancellationToken cancellationToken)
        {
            var player = message.Player;
            var myInfo = _mapper.Map<MyInfo>(player);
            await _mudProvider.UpdatePlayerInfo(player.Id, myInfo);

        }

        public async Task Handle(SendMessageEvent message, CancellationToken cancellationToken)
        {
            var playerId = message.PlayerId;
            var content = message.Content;
      
            await _queueHandler.SendQueueMessage(new SaveChatLogQueue(playerId, content));
            //await _mudProvider.ShowMessage(playerId, $"你说：{content}");

            /*
            await _chatLogDomainService.Add(new ChatLogEntity
            {
                PlayerId = playerId,
                Content = content,
                PostDate = DateTime.Now
            });


            await Commit();
            */
            // await _delayedQueue.Publish(new MessageModel { Content = receivedMessage.Content, PlayerId = _account.PlayerId }, 2, 10);

        }


        public async Task Handle(CompleteQuestEvent message, CancellationToken cancellationToken)
        {
            var player = message.Player;
            var quest = message.Quest;

            if(quest.Type== QuestTypeEnum.主线)
            {
                //await CheckPlayerMainQuest(player);
                await _queueHandler.SendQueueMessage(new CheckPlayerMainQuestQueue(player.Id));
            }
        }


        public async Task Handle(QuestTriggerEvent message, CancellationToken cancellationToken)
        {
            var player = message.Player;
            var questTriggerType = message.QuestTriggerType;

            //已经领取的所有任务
            var myQuests = (await _playerQuestDomainService.GetPlayerQuests(player.Id));
            //正在进行的任务
            var myQuestsNotComplete = myQuests.Where(x => !x.IsComplete);
            //所有未完成任务
            var quests = (await _questDomainService.GetAll()).Where(x => myQuestsNotComplete.Select(y => y.QuestId).Contains(x.Id));


            foreach(var quest in quests)
            {
                List<QuestTarget> questTargets = new List<QuestTarget>();
                try
                {
                    questTargets = JsonConvert.DeserializeObject<List<QuestTarget>>(quest.Target);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Convert QuestTarget:{ex}");
                }

                if (questTargets == null || questTargets.Count == 0)
                {
                    continue;
                }

                if (!questTargets.Exists(x => x.Target == questTriggerType.ToString()))
                {
                    continue;
                }

                foreach (var questTarget in questTargets)
                {
                    var npcId = questTarget.Attrs.FirstOrDefault(x => x.Attr == "NpcId")?.Val;
                    var questId = questTarget.Attrs.FirstOrDefault(x => x.Attr == "QuestId")?.Val;
                    int.TryParse(questTarget.Attrs.FirstOrDefault(x => x.Attr == "RoomId")?.Val, out int roomId);

                    var targetEnum = (QuestTargetEnum)Enum.Parse(typeof(QuestTargetEnum), questTarget.Target, true);
                    switch (targetEnum)
                    {

                        case QuestTargetEnum.与某个Npc对话:
    
                            break;

                        case QuestTargetEnum.所在房间:
                            if (player.RoomId != roomId)
                            {
                                continue;
                            }


                            break;

                    }
                }


            }


        }


        /*
        /// <summary>
        /// 检查并自动领取主线任务
        /// </summary>
        /// <returns></returns>
        private async Task CheckPlayerMainQuest(PlayerEntity player)
        {
            //已经领取的所有任务
            var myQuests = (await _playerQuestDomainService.GetPlayerQuests(player.Id));
            //正在进行的任务
            var myQuestsNotComplete = myQuests.Where(x => !x.IsComplete);
            //所有主线任务
            var mainQuests = (await _questDomainService.GetAll()).Where(x => x.Type == QuestTypeEnum.主线).OrderBy(x => x.SortId);
            //是否有正在进行的主线任务
            var mainQuest = mainQuests.FirstOrDefault(x => myQuestsNotComplete.Select(y => y.QuestId).Contains(x.Id));
            if (mainQuest == null)
            {
                //没有正在进行中的主线任务,找到第一个没有领取的主线任务
                mainQuest = mainQuests.FirstOrDefault(x => !myQuests.Select(y => y.QuestId).Contains(x.Id));
                if (mainQuest != null)
                {
                    //自动领取第一个主线任务
                    var playerQuest = new PlayerQuestEntity
                    {
                        PlayerId = player.Id,
                        QuestId = mainQuest.Id,
                        IsComplete = false,
                        TakeDate = DateTime.Now,
                        CompleteDate = DateTime.Now,
                        CreateDate = DateTime.Now,
                        DayTimes = 1,
                        HasTake = true,
                        Target = mainQuest.Target,
                        Times = 1,
                        UpdateDate = DateTime.Now
                    };
                    await _playerQuestDomainService.Add(playerQuest);

                    await _mudProvider.ShowMessage(player.Id, $"已自动激活任务 [{mainQuest.Name}]。");

                    //判断是否第一个任务
                    var isFirst = mainQuests.FirstOrDefault()?.Id == mainQuest.Id;


                    await _mudProvider.ShowQuest(player.Id, new { mainQuest, isFirst });
                }
                else
                {
                    //所有主线任务都已完成
                }

            }
            else
            {
                //有正在进行的主线任务

                //判断是否第一个任务
                var isFirst = mainQuests.FirstOrDefault()?.Id == mainQuest.Id;

                await _mudProvider.ShowQuest(player.Id, new { mainQuest, isFirst });
            }
        }
        */
    }
}
