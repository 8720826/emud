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
using Emprise.MudServer.Commands.EmailCommands;
using Emprise.Domain.Email.Services;
using Emprise.Domain.Email.Models;
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

namespace Emprise.MudServer.CommandHandlers
{
    
    public class QuestCommandHandler : CommandHandler, 
        IRequestHandler<QuestCommand, Unit>
        
    {
        private readonly IMediatorHandler _bus;
        private readonly ILogger<QuestCommandHandler> _logger;
        private readonly IQuestDomainService _questDomainService;
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
                if (playerQuest.HasTake)
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
                    IsComplete = false,
                    TakeDate = DateTime.Now,
                    CompleteDate = DateTime.Now,
                    CreateDate = DateTime.Now,
                    DayTimes = 1,
                    HasTake = true,
                    Target = quest.Target,
                    Times = 1,
                    UpdateDate = DateTime.Now
                };
                await _playerQuestDomainService.Add(playerQuest);

            }
            else
            {
                //TODO 领取任务
                playerQuest.HasTake = true;
                playerQuest.IsComplete = false;
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
    }
}
