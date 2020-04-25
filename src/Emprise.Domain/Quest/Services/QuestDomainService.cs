using Emprise.Domain.Common.Modes;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enum;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Npc.Entity;
using Emprise.Domain.Quest.Entity;
using Emprise.Domain.Quest.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Emprise.Domain.Quest.Services
{
    public class QuestDomainService : IQuestDomainService
    {
        private readonly IRepository<QuestEntity> _questRepository;
        private readonly IMemoryCache _cache;
        private readonly IMediatorHandler _bus;
        private readonly ILogger<QuestDomainService> _logger;
        public QuestDomainService(IRepository<QuestEntity> questRepository, IMemoryCache cache, IMediatorHandler bus, ILogger<QuestDomainService> logger)
        {
            _questRepository = questRepository;
            _cache = cache;
            _bus = bus;
            _logger = logger;
        }


        public async Task<QuestEntity> Get(Expression<Func<QuestEntity, bool>> where)
        {
            return await _questRepository.Get(where);
        }

        public async Task<List<QuestEntity>> GetAll()
        {
           
            var key = CacheKey.QuestList;
            return await _cache.GetOrCreateAsync(key, async p => {
                p.SetAbsoluteExpiration(TimeSpan.FromMinutes(60));
                return await Task.Run(async () =>
                {
                    var query = await _questRepository.GetAll();
                    return query.ToList();
                });
            });
  
        }

        public async Task<QuestEntity> Get(int id)
        {
            var key = string.Format(CacheKey.Quest, id);
            return await _cache.GetOrCreateAsync(key, async p => {
                p.SetAbsoluteExpiration(TimeSpan.FromMinutes(60));
                return await Task.Run(async () =>
                {
                    return await _questRepository.Get(id);
                });
            });
        }

        public async Task Add(QuestEntity room)
        {
            await _questRepository.Add(room);
        }

        public async Task Update(QuestEntity room)
        {
            await _questRepository.Update(room);
            await _bus.RaiseEvent(new EntityUpdatedEvent<QuestEntity>(room)).ConfigureAwait(false);
        }



        /// <summary>
        /// 检查是否触发
        /// </summary>
        /// <param name="triggerTypeEnum"></param>
        /// <param name="checkModel"></param>
        /// <returns></returns>
        public async Task<QuestEntity> CheckTriggerCondition(QuestTriggerTypeEnum triggerTypeEnum, QuestTriggerCheckModel checkModel)
        {
            var quests = (await GetAll()).Where(x => x.TriggerType == triggerTypeEnum).ToList();
            _logger.LogInformation($"quests.Count={quests.Count}");
            foreach (var quest in quests)
            {
                _logger.LogInformation($"quest={quest.Id}");
                List<QuestTrigger> taskTriggers = new List<QuestTrigger>();
                try
                {
                    taskTriggers = JsonConvert.DeserializeObject<List<QuestTrigger>>(quest.TriggerCondition);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Convert TaskTrigger:{ex}");
                }

                if (taskTriggers == null || taskTriggers.Count == 0)
                {
                    continue;
                }

                var taskTrigger = taskTriggers.FirstOrDefault();
                if (taskTrigger == null || taskTrigger.Attrs == null || taskTrigger.Attrs.Count == 0)
                {
                    continue;
                }


                int.TryParse(taskTrigger.Attrs.FirstOrDefault(x => x.Attr == "NpcId")?.Val,out int npcId);

                _logger.LogInformation($"npcId={npcId}");
                switch (triggerTypeEnum)
                {
                    case QuestTriggerTypeEnum.与Npc对话:
                    case QuestTriggerTypeEnum.杀死Npc:
                        if (checkModel.NpcId == npcId)
                        {
                            return quest;
                        }


                        break;
                }


            }

            return null;
        }



        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
