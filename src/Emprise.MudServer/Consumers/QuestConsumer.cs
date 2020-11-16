using DotNetCore.CAP;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Interfaces.Ioc;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.Player.Services;
using Emprise.Domain.Quest.Entity;
using Emprise.Domain.Quest.Models;
using Emprise.Domain.Quest.Services;
using Emprise.Domain.Ware.Services;
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


        Task<bool> CheckPlayerNewbieQuestQueue(CheckPlayerNewbieQuestQueue message);

        Task<bool> CompleteQuestNewbieQuestQueue(CompleteQuestNewbieQuestQueue message);
    }

    public class QuestConsumer : BaseConsumer, IQuestConsumer, ICapSubscribe
    {
        private readonly ILogger<ChatConsumer> _logger;
        private readonly IPlayerQuestDomainService _playerQuestDomainService;
        private readonly IQuestDomainService _questDomainService;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly IWareDomainService _wareDomainService;
        private readonly IMudProvider _mudProvider;
  

        public QuestConsumer(
            IPlayerQuestDomainService playerQuestDomainService,
            IQuestDomainService questDomainService,
            IPlayerDomainService playerDomainService,
            IWareDomainService wareDomainService,
            IMudProvider mudProvider,
            ILogger<ChatConsumer> logger, IUnitOfWork uow, IRedisDb redisDb) : base(uow, redisDb)
        {
            _logger = logger;
            _playerQuestDomainService = playerQuestDomainService;
            _questDomainService = questDomainService;
            _playerDomainService = playerDomainService;
            _wareDomainService = wareDomainService;
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
            var myQuestsNotComplete = myQuests.Where(x => x.Status== QuestStateEnum.已领取进行中);
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
                        Status = QuestStateEnum.已领取进行中,
                        //IsComplete = false,
                        TakeDate = DateTime.Now,
                        CompleteDate = DateTime.Now,
                        CreateDate = DateTime.Now,
                        DayTimes = 1,
                        //HasTake = true,
                        Target = mainQuest.Target,
                        Times = 1,
                        UpdateDate = DateTime.Now
                    };
                    await _playerQuestDomainService.Add(playerQuest);

                    await _mudProvider.ShowMessage(playerId, $"已自动激活任务 [{mainQuest.Name}]。");

                    //判断是否第一个任务
                    var isFirst = mainQuests.FirstOrDefault()?.Id == mainQuest.Id;


                    await _mudProvider.ShowMainQuest(playerId, new { mainQuest, isFirst });
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

                await _mudProvider.ShowMainQuest(playerId, new { mainQuest, isFirst });
            }


            await Commit();

            return true;
        }



        [CapSubscribe("CheckPlayerNewbieQuestQueue")]
        public async Task<bool> CheckPlayerNewbieQuestQueue(CheckPlayerNewbieQuestQueue message)
        {
            _logger.LogDebug($"Consumer Get Queue {JsonConvert.SerializeObject(message)} ready");

            var playerId = message.PlayerId;

            //已经领取的所有任务
            var myQuests = (await _playerQuestDomainService.GetPlayerQuests(playerId));

            //正在进行的任务
            var myQuestsNotComplete = myQuests.Where(x => x.Status == QuestStateEnum.已领取进行中);
            //所有新手任务
            var newbieQuests = (await _questDomainService.GetAll()).Where(x => x.Type == QuestTypeEnum.新手).ToList();
            foreach (var newbieQuest in newbieQuests)
            {
                if(myQuests.Count(x=>x.QuestId== newbieQuest.Id) == 0)
                {
                    var playerQuest = new PlayerQuestEntity
                    {
                        PlayerId = playerId,
                        QuestId = newbieQuest.Id,
                        Status = QuestStateEnum.已领取进行中,
                        TakeDate = DateTime.Now,
                        CompleteDate = DateTime.Now,
                        CreateDate = DateTime.Now,
                        DayTimes = 1,
                        Target = newbieQuest.Target,
                        Times = 1,
                        UpdateDate = DateTime.Now
                    };
                    await _playerQuestDomainService.Add(playerQuest);
                }
            }


            await Commit();
            return true;
        }



        [CapSubscribe("CompleteQuestNewbieQuestQueue")]
        public async Task<bool> CompleteQuestNewbieQuestQueue(CompleteQuestNewbieQuestQueue message)
        {
            _logger.LogDebug($"Consumer Get Queue {JsonConvert.SerializeObject(message)} ready");

            var playerId = message.PlayerId;
            var questType =  message.Quest;

            int questId = 0;
            switch (questType)
            {
                case NewbieQuestEnum.第一次伐木:
                    questId = 5;
                    break;

                case NewbieQuestEnum.第一次聊天:
                    questId = 3;
                    break;
            }
            if (questId == 0)
            {
                return true;
            }

            var hasCompleteQuest = await _redisDb.StringGet<int>(string.Format(RedisKey.CompleteQuest, playerId, questId));
            if (hasCompleteQuest==1)
            {
                return true;
            }

            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                return true;
            }

            var quest = await _questDomainService.Get(questId);
            if (quest == null || quest.Type != QuestTypeEnum.新手)
            {
                return true;
            }

            var playerQuest = await _playerQuestDomainService.Get(x => x.PlayerId == playerId && x.QuestId == quest.Id);
            if (playerQuest == null || playerQuest.Status != QuestStateEnum.已领取进行中)
            {
                return true;
            }



            //TODO 修改任务状态
            //playerQuest.HasTake = false;
            playerQuest.CompleteDate = DateTime.Now;
            playerQuest.Status = QuestStateEnum.完成已领奖;
            playerQuest.CompleteTimes++;
            //playerQuest.IsComplete = true;
            await _playerQuestDomainService.Update(playerQuest);

            await _mudProvider.ShowMessage(player.Id, $"完成任务[{quest.Name}]");

            await TakeQuestReward(player, quest.Reward);
            //TODO  领取奖励



            if (await Commit())
            {
                await _redisDb.StringSet<int>(string.Format(RedisKey.CompleteQuest, playerId, questId),1);
            }

            return true;
        }

        /*
        public async Task<Unit> Handle(CompleteQuestCommand command, CancellationToken cancellationToken)
        {

            var playerId = command.PlayerId;
            var questId = command.QuestId;

            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            var quest = await _questDomainService.Get(questId);
            if (quest == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"任务不存在！"));
                return Unit.Value;
            }

            var playerQuest = await _playerQuestDomainService.Get(x => x.PlayerId == playerId && x.QuestId == quest.Id);
            if (playerQuest == null)
            {
                await _mudProvider.ShowMessage(playerId, "请先领取任务！");
                return Unit.Value;
            }

            //未领取
            if (playerQuest.Status != QuestStateEnum.已领取进行中)
            {
                await _mudProvider.ShowMessage(playerId, "请先领取任务！");
                return Unit.Value;
            }

            var checkResult = await CheckQuestIfComplete(player, playerQuest.Target);
            if (!checkResult.IsSuccess)
            {
                //任务未完成
                await _mudProvider.ShowMessage(player.Id, "任务未完成！");
                return Unit.Value;
            }

            var checkConsumeResult = await CheckQuestConsume(player, quest.Consume);

            if (!checkConsumeResult.IsSuccess)
            {
                await _mudProvider.ShowMessage(player.Id, $"领取奖励失败！{checkConsumeResult.ErrorMessage}");
                return Unit.Value;
            }



            //TODO 修改任务状态
            //playerQuest.HasTake = false;
            playerQuest.CompleteDate = DateTime.Now;
            playerQuest.Status = QuestStateEnum.完成已领奖;
            playerQuest.CompleteTimes++;
            //playerQuest.IsComplete = true;
            await _playerQuestDomainService.Update(playerQuest);

            await DoQuestConsume(player, quest.Consume);


            await TakeQuestReward(player, quest.Reward);
            //TODO  领取奖励



            if (await Commit())
            {
                //await _mudProvider.ShowMessage(player.Id, quest.CompletedWords);
                await _mudProvider.ShowMessage(player.Id, "领取奖励成功");

                await _bus.RaiseEvent(new CompleteQuestEvent(player, quest)).ConfigureAwait(false);
            }

            return Unit.Value;
        }
         */


        /// <summary>
        /// 领取奖励
        /// </summary>
        /// <param name="player"></param>
        /// <param name="rewardStr"></param>
        /// <returns></returns>
        private async Task<ResultModel> TakeQuestReward(PlayerEntity player, string rewardStr)
        {
            var result = new ResultModel
            {
                IsSuccess = false
            };
            if (!string.IsNullOrEmpty(rewardStr))
            {
                List<QuestReward> questRewards = new List<QuestReward>();
                try
                {
                    questRewards = JsonConvert.DeserializeObject<List<QuestReward>>(rewardStr);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Convert QuestReward:{ex}");
                }

                foreach (var questReward in questRewards)
                {
                    int.TryParse(questReward.Attrs.FirstOrDefault(x => x.Attr == "NpcId")?.Val, out int npcId);
                    int.TryParse(questReward.Attrs.FirstOrDefault(x => x.Attr == "Exp")?.Val, out int exp);
                    long.TryParse(questReward.Attrs.FirstOrDefault(x => x.Attr == "Money")?.Val, out long money);
                    int.TryParse(questReward.Attrs.FirstOrDefault(x => x.Attr == "WareId")?.Val, out int wareId);
                    int.TryParse(questReward.Attrs.FirstOrDefault(x => x.Attr == "Number")?.Val, out int number);

                    var rewardEnum = (QuestRewardEnum)Enum.Parse(typeof(QuestRewardEnum), questReward.Reward, true);


                    switch (rewardEnum)
                    {


                        case QuestRewardEnum.物品:
                            var ware = await _wareDomainService.Get(wareId);
                            if (ware != null)
                            {
                                await _mudProvider.ShowMessage(player.Id, $"获得 {number}{ware.Unit}[{ware.Name}]");
                            }
                            // 添加物品

                            break;

                        case QuestRewardEnum.经验:
                            player.Exp += exp;
                            await _mudProvider.ShowMessage(player.Id, $"获得经验 +{exp}");
                            break;

                        case QuestRewardEnum.金钱:
                            player.Money += money;
                            await _mudProvider.ShowMessage(player.Id, $"获得 +{money.ToMoney()}");
                            break;


                    }

                }

                await _playerDomainService.Update(player);
            }

            result.IsSuccess = true;
            return await Task.FromResult(result);
        }
    }
}
