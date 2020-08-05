using AutoMapper;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.CommandHandlers;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Notifications;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Emprise.Domain.Quest.Entity;
using System;
using Emprise.Domain.Core.Enums;
using Emprise.MudServer.Commands;
using Emprise.Domain.Quest.Models;
using System.Collections.Generic;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.Player.Services;
using Emprise.Domain.Quest.Services;
using Emprise.MudServer.Commands.QuestCommands;
using Emprise.MudServer.Events;
using Emprise.Domain.Ware.Services;

namespace Emprise.MudServer.CommandHandlers
{
    
    public class QuestCommandHandler : CommandHandler, 
        IRequestHandler<QuestCommand, Unit>,
        IRequestHandler<ShowMyQuestCommand, Unit>,
        IRequestHandler<ShowMyHistoryQuestCommand, Unit>,
        IRequestHandler<ShowQuestDetailCommand, Unit>
        
    {
        private readonly IMediatorHandler _bus;
        private readonly ILogger<QuestCommandHandler> _logger;
        private readonly IQuestDomainService _questDomainService;
        private readonly IWareDomainService _wareDomainService;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly IMapper _mapper;
        private readonly IMail _mail;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly IPlayerQuestDomainService _playerQuestDomainService;
        private readonly IRedisDb _redisDb;
        private readonly IMudProvider _mudProvider;


        public QuestCommandHandler(
            IMediatorHandler bus,
            ILogger<QuestCommandHandler> logger,
            IQuestDomainService questDomainService,
            IWareDomainService wareDomainService,
            IHttpContextAccessor httpAccessor,
            IMapper mapper,
            IMail mail,
            IPlayerDomainService playerDomainService,
            IPlayerQuestDomainService playerQuestDomainService,
            IRedisDb redisDb,
            IMudProvider mudProvider,
            INotificationHandler<DomainNotification> notifications,
            IUnitOfWork uow) : base(uow, bus, notifications)
        {

            _bus = bus;
            _logger = logger;
            _questDomainService = questDomainService;
            _wareDomainService = wareDomainService;
            _httpAccessor = httpAccessor;
            _mapper = mapper;
            _mail = mail;
            _playerDomainService = playerDomainService;
            _playerQuestDomainService = playerQuestDomainService;
            _redisDb = redisDb;
            _mudProvider = mudProvider;
        }

        public async Task<Unit> Handle(QuestCommand command, CancellationToken cancellationToken)
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

            var playerQuest = await _playerQuestDomainService.Get(x => x.PlayerId == playerId && x.QuestId == questId);
            if (playerQuest != null)
            {
                //已领取
                if (playerQuest.Status == QuestStateEnum.已领取进行中)
                {
                    await _mudProvider.ShowMessage(playerId, "领取任务成功！");
                    //await _mudProvider.ShowMessage(playerId, quest.InProgressWords);
                    return Unit.Value;
                }

                //未领取但是之前已经完成过
                switch (quest.Period)
                {
                    case QuestPeriodEnum.不可重复:
                        await _mudProvider.ShowMessage(playerId, "该任务仅可领取一次，你已经领取过！");
                        return Unit.Value;
                        break;

                    case QuestPeriodEnum.无限制:

                        break;

                    case QuestPeriodEnum.每周一次:
                        if (DateTime.Now.Subtract(playerQuest.TakeDate).TotalDays <= 7)
                        {
                            await _mudProvider.ShowMessage(playerId, "该任务每周仅可领取一次，你已经领取过！");
                            return Unit.Value;
                        }
                        break;

                    case QuestPeriodEnum.每天一次:
                        if (DateTime.Now.Subtract(playerQuest.TakeDate).TotalHours <= 24)
                        {
                            await _mudProvider.ShowMessage(playerId, "该任务每天仅可领取一次，你已经领取过！");
                            return Unit.Value;
                        }
                        break;

                    case QuestPeriodEnum.每年一次:
                        if (DateTime.Now.Subtract(playerQuest.TakeDate).TotalDays <= 365)
                        {
                            await _mudProvider.ShowMessage(playerId, "该任务每年仅可领取一次，你已经领取过！");
                            return Unit.Value;
                        }
                        break;

                    case QuestPeriodEnum.每月一次:
                        if (DateTime.Now.Subtract(playerQuest.TakeDate).TotalDays <= 30)
                        {
                            await _mudProvider.ShowMessage(playerId, "该任务每月仅可领取一次，你已经领取过！");
                            return Unit.Value;
                        }
                        break;
                }

            }

            var checkCondition = await CheckTakeCondition(player, quest.TakeCondition);
            if (!checkCondition.IsSuccess)
            {
                await _bus.RaiseEvent(new DomainNotification($"你还不能领取这个任务 ！{checkCondition.ErrorMessage}"));
                return Unit.Value;
            }

            if (playerQuest == null)
            {
                playerQuest = new PlayerQuestEntity
                {
                    PlayerId = player.Id,
                    QuestId = questId,
                    //IsComplete = false,
                    Status = QuestStateEnum.已领取进行中,
                    TakeDate = DateTime.Now,
                    CompleteDate = DateTime.Now,
                    CreateDate = DateTime.Now,
                    DayTimes = 1,
                    //HasTake = true,
                    Target = quest.Target,
                    Times = 1,
                    UpdateDate = DateTime.Now
                };
                await _playerQuestDomainService.Add(playerQuest);

            }
            else
            {
                //TODO 领取任务
                //playerQuest.HasTake = true;
                //playerQuest.IsComplete = false;

                playerQuest.Status = QuestStateEnum.已领取进行中;
                playerQuest.TakeDate = DateTime.Now;
                playerQuest.Times += 1;
                playerQuest.Target = quest.Target;

                await _playerQuestDomainService.Update(playerQuest);
            }


            if (await Commit())
            {
                await _bus.RaiseEvent(new DomainNotification($"领取任务 {quest.Name} ！"));
            }



            return Unit.Value;
        }

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

        public async Task<Unit> Handle(ShowMyQuestCommand command, CancellationToken cancellationToken)
        {
            var playerId = command.PlayerId;

            var playerQuests = await _playerQuestDomainService.GetPlayerQuests(playerId);

            var ids = playerQuests.Where(x => x.Status == QuestStateEnum.已领取进行中).Select(x => x.QuestId).ToList();

            var questQuery = await _questDomainService.GetAll();

            var quests = questQuery.Where(x => ids.Contains(x.Id));



            await _mudProvider.ShowQuests(playerId, _mapper.Map<List<QuestModel>>(quests));

            return Unit.Value;
        }

        public async Task<Unit> Handle(ShowMyHistoryQuestCommand command, CancellationToken cancellationToken)
        {
            var playerId = command.PlayerId;


            var playerQuests = await _playerQuestDomainService.GetPlayerQuests(playerId);

            var ids = playerQuests.Where(x => x.CompleteTimes > 0).Select(x => x.QuestId).ToList();

            var questQuery = await _questDomainService.GetAll();

            var quests = questQuery.Where(x => ids.Contains(x.Id));

            await _mudProvider.ShowHistoryQuests(playerId, _mapper.Map<List<QuestModel>>(quests));

            return Unit.Value;
        }


        
        public async Task<Unit> Handle(ShowQuestDetailCommand command, CancellationToken cancellationToken)
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



            await _mudProvider.ShowQuest(playerId, _mapper.Map<QuestModel>(quest));

            return Unit.Value;
        }


        /// <summary>
        /// 判断用户是否可以领取某个任务
        /// </summary>
        /// <param name="player"></param>
        /// <param name="takeConditionStr"></param>
        /// <returns></returns>
        private async Task<ResultModel> CheckTakeCondition(PlayerEntity player, string takeConditionStr)
        {
            var result = new ResultModel { IsSuccess = false };

            if (!string.IsNullOrEmpty(takeConditionStr))
            {
                List<QuestTakeCondition> takeConditions = new List<QuestTakeCondition>();
                try
                {
                    takeConditions = JsonConvert.DeserializeObject<List<QuestTakeCondition>>(takeConditionStr);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Convert CaseIf:{ex}");
                }

                foreach (var takeCondition in takeConditions)
                {
                    var npcId = takeCondition.Attrs.FirstOrDefault(x => x.Attr == "NpcId")?.Val;
                    var questId = takeCondition.Attrs.FirstOrDefault(x => x.Attr == "QuestId")?.Val;
                    int.TryParse(takeCondition.Attrs.FirstOrDefault(x => x.Attr == "RoomId")?.Val, out int roomId);

                    var targetEnum = (QuestTakeConditionEnum)Enum.Parse(typeof(QuestTakeConditionEnum), takeCondition.Condition, true);


                    switch (targetEnum)
                    {


                        case QuestTakeConditionEnum.与某个Npc对话:
                            if (await _redisDb.StringGet<int>(string.Format(RedisKey.ChatWithNpc, player.Id, npcId)) != 1)
                            {
                                result.ErrorMessage = $"";
                                return result;
                            }
                            break;

                        case QuestTakeConditionEnum.完成前置任务:

                            if (await _redisDb.StringGet<int>(string.Format(RedisKey.CompleteQuest, player.Id, questId)) != 1)
                            {
                                result.ErrorMessage = $"";
                                return result;
                            }

                            break;


                    }
                }
            }




            result.IsSuccess = true;
            return result;
        }


        /// <summary>
        /// 判断用户是否满足完成任务的条件
        /// </summary>
        /// <param name="player"></param>
        /// <param name="targetStr"></param>
        /// <returns></returns>
        private async Task<ResultModel> CheckQuestIfComplete(PlayerEntity player, string targetStr)
        {
            var result = new ResultModel { IsSuccess = false };

            if (!string.IsNullOrEmpty(targetStr))
            {
                List<QuestTarget> questTargets = new List<QuestTarget>();
                try
                {
                    questTargets = JsonConvert.DeserializeObject<List<QuestTarget>>(targetStr);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Convert QuestTarget:{ex}");
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
                            if (await _redisDb.StringGet<int>(string.Format(RedisKey.ChatWithNpc, player.Id, npcId)) != 1)
                            {
                                result.ErrorMessage = $"";
                                return result;
                            }
                            break;

                        case QuestTargetEnum.所在房间:
                            if (player.RoomId != roomId)
                            {
                                result.ErrorMessage = "";
                                return result;
                            }


                            break;

                    }
                }
            }





            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 检查消耗
        /// </summary>
        /// <param name="player"></param>
        /// <param name="consumeStr"></param>
        /// <returns></returns>
        private async Task<ResultModel> CheckQuestConsume(PlayerEntity player, string consumeStr)
        {
            var result = new ResultModel
            {
                IsSuccess = false
            };
            if (!string.IsNullOrEmpty(consumeStr))
            {
                List<QuestConsume> questConsumes = new List<QuestConsume>();
                try
                {
                    questConsumes = JsonConvert.DeserializeObject<List<QuestConsume>>(consumeStr);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Convert QuestConsume:{ex}");
                }

                foreach (var questConsume in questConsumes)
                {
                    var npcId = questConsume.Attrs.FirstOrDefault(x => x.Attr == "NpcId")?.Val;
                    int.TryParse(questConsume.Attrs.FirstOrDefault(x => x.Attr == "Exp")?.Val, out int exp);
                    long.TryParse(questConsume.Attrs.FirstOrDefault(x => x.Attr == "Money")?.Val, out long money);

                    var consumeEnum = (QuestConsumeEnum)Enum.Parse(typeof(QuestConsumeEnum), questConsume.Consume, true);


                    switch (consumeEnum)
                    {


                        case QuestConsumeEnum.物品:

                            //TODO
                            result.ErrorMessage = $"完成任务需要消耗物品";
                            return result;
                            break;

                        case QuestConsumeEnum.经验:
                            if (player.Exp < exp)
                            {
                                result.ErrorMessage = $"完成任务需要消耗经验 {exp}";
                                return result;
                            }
                            break;

                        case QuestConsumeEnum.金钱:
                            if (player.Money < money)
                            {
                                result.ErrorMessage = $"完成任务需要消耗 {money.ToMoney()}";
                                return result;
                            }

                            break;


                    }

                }
            }

            result.IsSuccess = true;
            return await Task.FromResult(result);
        }

        /// <summary>
        /// 执行消耗
        /// </summary>
        /// <param name="player"></param>
        /// <param name="consumeStr"></param>
        /// <returns></returns>
        private async Task<ResultModel> DoQuestConsume(PlayerEntity player, string consumeStr)
        {
            var result = new ResultModel
            {
                IsSuccess = false
            };
            if (!string.IsNullOrEmpty(consumeStr))
            {
                List<QuestConsume> questConsumes = new List<QuestConsume>();
                try
                {
                    questConsumes = JsonConvert.DeserializeObject<List<QuestConsume>>(consumeStr);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Convert QuestConsume:{ex}");
                }

                foreach (var questConsume in questConsumes)
                {
                    var npcId = questConsume.Attrs.FirstOrDefault(x => x.Attr == "NpcId")?.Val;
                    int.TryParse(questConsume.Attrs.FirstOrDefault(x => x.Attr == "Exp")?.Val, out int exp);
                    long.TryParse(questConsume.Attrs.FirstOrDefault(x => x.Attr == "Money")?.Val, out long money);

                    var consumeEnum = (QuestConsumeEnum)Enum.Parse(typeof(QuestConsumeEnum), questConsume.Consume, true);


                    switch (consumeEnum)
                    {


                        case QuestConsumeEnum.物品:

                            //TODO 减少物品
                            return result;
                            break;

                        case QuestConsumeEnum.经验:
                            player.Exp -= exp;
                            break;

                        case QuestConsumeEnum.金钱:
                            player.Money -= money;
                            break;


                    }

                }

                await _playerDomainService.Update(player);
            }

            result.IsSuccess = true;
            return await Task.FromResult(result);
        }


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
                                await _mudProvider.ShowMessage(player.Id, $"获得 [{ware.Name}] X{number}");
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
