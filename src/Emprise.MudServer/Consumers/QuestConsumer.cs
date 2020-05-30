using DotNetCore.CAP;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Interfaces.Ioc;
using Emprise.Domain.Quest.Entity;
using Emprise.Domain.Quest.Services;
using Emprise.MudServer.Queues;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.MudServer.Consumers
{
    public interface IQuestConsumer : ITransient
    {
        Task<bool> CheckPlayerMainQuestQueue(CheckPlayerMainQuestQueue message);

    }

    public class QuestConsumer : BaseConsumer, IQuestConsumer, ICapSubscribe
    {
        private readonly ILogger<ChatConsumer> _logger;
        private readonly IPlayerQuestDomainService _playerQuestDomainService;
        private readonly IQuestDomainService _questDomainService;
        private readonly IMudProvider _mudProvider;


        public QuestConsumer(
            IPlayerQuestDomainService playerQuestDomainService,
            IQuestDomainService questDomainService,
            IMudProvider mudProvider,
            ILogger<ChatConsumer> logger, IUnitOfWork uow) : base(uow)
        {
            _logger = logger;
            _playerQuestDomainService = playerQuestDomainService;
            _questDomainService = questDomainService;
            _mudProvider = mudProvider;
        }

        [CapSubscribe("CheckPlayerMainQuestQueue")]
        public async Task<bool> CheckPlayerMainQuestQueue(CheckPlayerMainQuestQueue message)
        {
            _logger.LogDebug($"Consumer Get Queue {JsonConvert.SerializeObject(message)} ready");

            var playerId = message.PlayerId;

            //已经领取的所有任务
            var myQuests = (await _playerQuestDomainService.GetPlayerQuests(playerId));
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
                        PlayerId = playerId,
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

                    await _mudProvider.ShowMessage(playerId, $"已自动激活任务 [{mainQuest.Name}]。");

                    //判断是否第一个任务
                    var isFirst = mainQuests.FirstOrDefault()?.Id == mainQuest.Id;


                    await _mudProvider.ShowQuest(playerId, new { mainQuest, isFirst });
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

                await _mudProvider.ShowQuest(playerId, new { mainQuest, isFirst });
            }


            await Commit();

            return true;
        }
    }
}
