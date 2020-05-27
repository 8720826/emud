using Emprise.Domain.Common.Modes;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Npc.Entity;
using Emprise.Domain.Quest.Services;
using Emprise.MudServer.Events;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emprise.MudServer.EventHandlers
{
    public  class NpcEventHandler :
        INotificationHandler<EntityUpdatedEvent<NpcEntity>>,
        INotificationHandler<EntityInsertedEvent<NpcEntity>>,
        INotificationHandler<EntityDeletedEvent<NpcEntity>>,
        INotificationHandler<ChatWithNpcEvent>
    {
        private readonly IQuestDomainService _questDomainService;
        private readonly IPlayerQuestDomainService _playerQuestDomainService;
        private readonly IMudProvider _mudProvider;
        private readonly ILogger<NpcEventHandler> _logger; 
        private readonly IRedisDb _redisDb;
        private readonly IMemoryCache _cache;

        public NpcEventHandler(ILogger<NpcEventHandler> logger, 
            IMudProvider mudProvider, 
            IQuestDomainService questDomainService, 
            IPlayerQuestDomainService playerQuestDomainService,
            IMemoryCache cache,
            IRedisDb redisDb)
        {
            _logger = logger;
            _mudProvider = mudProvider;
            _questDomainService = questDomainService;
            _playerQuestDomainService = playerQuestDomainService;
            _redisDb = redisDb;
            _cache = cache;
        }

        public async Task Handle(EntityUpdatedEvent<NpcEntity> message, CancellationToken cancellationToken)
        {
            var key = string.Format(CacheKey.Npc, message.Entity.Id);

            _logger.LogInformation($"{key}");

            await Task.Run(() => {
                _cache.Remove(key);
            });
        }

        public async Task Handle(EntityInsertedEvent<NpcEntity> message, CancellationToken cancellationToken)
        {
            var key = string.Format(CacheKey.Npc, message.Entity.Id);
            await Task.Run(() => {
                _cache.Set(key, message.Entity);
            });
        }

        public async Task Handle(EntityDeletedEvent<NpcEntity> message, CancellationToken cancellationToken)
        {
            var key = string.Format(CacheKey.Npc, message.Entity.Id);
            await Task.Run(() => {
                _cache.Remove(key);
            });
        }

        public async Task Handle(ChatWithNpcEvent message, CancellationToken cancellationToken)
        {
            var playerId = message.PlayerId;
            var npcId = message.NpcId;


            await _redisDb.StringSet<int>(string.Format(RedisKey.ChatWithNpc, playerId, npcId), 1, DateTime.Now.AddDays(30));




            /*
            _logger.LogInformation($"CheckQuest  playerId={playerId},{npcId}");

            var quests = (await _questDomainService.GetAll()).Where(x=>x.NpcId== npcId).ToList();
            if (quests.Count == 0)
            {
                return;
            }



            bool canTake = false;

            var playerQuest = await _playerQuestDomainService.Get(x => x.PlayerId == playerId && x.QuestId == quest.Id);
            if (playerQuest == null)
            {
                canTake = true;
            }
            else
            {
                if (playerQuest.IsComplete)
                {
                    switch (quest.Period)
                    {
                        case QuestPeriodEnum.不可重复: break;

                        case QuestPeriodEnum.无限制:
                            canTake = true;
                            break;

                        case QuestPeriodEnum.每周一次:
                            if (DateTime.Now.Subtract(playerQuest.TakeDate).TotalDays > 7)
                            {
                                canTake = true;
                            }
                            break;

                        case QuestPeriodEnum.每天一次:
                            if (DateTime.Now.Subtract(playerQuest.TakeDate).TotalHours > 24)
                            {
                                canTake = true;
                            }
                            break;

                        case QuestPeriodEnum.每年一次:
                            if (DateTime.Now.Subtract(playerQuest.TakeDate).TotalDays > 365)
                            {
                                canTake = true;
                            }
                            break;

                        case QuestPeriodEnum.每月一次:
                            if (DateTime.Now.Subtract(playerQuest.TakeDate).TotalDays > 30)
                            {
                                canTake = true;
                            }
                            break;
                    }
                }
                else
                {
                    await _mudProvider.ShowMessage(playerId, quest.InProgressWords);

                    await _mudProvider.ShowMessage(playerId, $"是否领取该任务 [{quest.Name}]?  <button type = 'button' class='quest' style='padding:1px 3px;' questId='{quest.Id}'> 确定 </button> ", MessageTypeEnum.指令);
                }

            }

            if (canTake)
            {
                _logger.LogInformation($"CheckQuest questId= {quest.Id}");
                await _mudProvider.ShowMessage(playerId, quest.BeforeCreate);

                await _mudProvider.ShowMessage(playerId, $"是否领取该任务 [{quest.Name}]?  <button type = 'button' class='quest' style='padding:1px 3px;' questId='{quest.Id}'> 确定 </button> ", MessageTypeEnum.指令);
            }
            */
        }
    }
}
