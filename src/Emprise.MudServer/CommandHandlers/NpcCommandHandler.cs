using AutoMapper;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.CommandHandlers;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Notifications;
using Emprise.MudServer.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Emprise.MudServer.Models;
using Emprise.Domain.Npc.Services;
using Emprise.MudServer.Events;
using Emprise.Domain.Player.Services;
using Microsoft.Extensions.Caching.Memory;
using Emprise.MudServer.Commands.NpcActionCommands;
using Emprise.Domain.Npc.Entity;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Npc.Models;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.Quest.Models;
using Emprise.Domain.Core.Attributes;
using Emprise.Domain.Ware.Services;
using Emprise.Domain.Quest.Entity;
using Emprise.Domain.Quest.Services;
using System.Reflection;
using Emprise.MudServer.Commands.NpcCommands;
using Emprise.Domain.NpcRelation.Services;
using Emprise.Domain.NpcRelation.Entity;

namespace Emprise.MudServer.CommandHandlers
{
    
    public class NpcCommandHandler : CommandHandler,
        IRequestHandler<ShowNpcCommand, Unit>,
        IRequestHandler<KillNpcCommand, Unit>,
        IRequestHandler<GiveToNpcCommand, Unit>,
        IRequestHandler<NpcScriptCommand, Unit>,
        IRequestHandler<ChatWithNpcCommand, Unit>
        

    {
        private readonly IMediatorHandler _bus;
        private readonly ILogger<NpcCommandHandler> _logger;
        private readonly INpcDomainService _npcDomainService;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly IScriptDomainService _scriptDomainService;
        private readonly INpcScriptDomainService _npcScriptDomainService;
        private readonly IScriptCommandDomainService _scriptCommandDomainService;
        private readonly IWareDomainService _wareDomainService;
        private readonly IPlayerWareDomainService _playerWareDomainService;
        private readonly IQuestDomainService _questDomainService;
        private readonly IPlayerQuestDomainService _playerQuestDomainService;
        private readonly INpcRelationDomainService _npcRelationDomainService;

        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly IRedisDb _redisDb;
        private readonly IMudProvider _mudProvider;


        public NpcCommandHandler(
            IMediatorHandler bus,
            ILogger<NpcCommandHandler> logger,
            INpcDomainService npcDomainService,
            IPlayerDomainService playerDomainService,
            IScriptDomainService scriptDomainService,
            INpcScriptDomainService npcScriptDomainService,
            IScriptCommandDomainService scriptCommandDomainService,
            IWareDomainService wareDomainService,
            IPlayerWareDomainService playerWareDomainService,
            IQuestDomainService questDomainService,
            IPlayerQuestDomainService playerQuestDomainService,
            INpcRelationDomainService npcRelationDomainService,
            IMapper mapper,
            IMemoryCache cache, 
            IRedisDb redisDb,
            IMudProvider mudProvider,
            INotificationHandler<DomainNotification> notifications,
            IUnitOfWork uow) : base(uow, bus, notifications)
        {

            _bus = bus;
            _logger = logger;
            _npcDomainService = npcDomainService;
            _mapper = mapper;
            _cache = cache;
            _playerDomainService = playerDomainService;
            _scriptDomainService = scriptDomainService;
            _npcScriptDomainService = npcScriptDomainService;
            _scriptCommandDomainService = scriptCommandDomainService;
            _wareDomainService = wareDomainService;
            _playerWareDomainService = playerWareDomainService;
            _questDomainService = questDomainService;
            _playerQuestDomainService = playerQuestDomainService;
            _npcRelationDomainService = npcRelationDomainService;
            _redisDb = redisDb;
            _mudProvider = mudProvider;
        }

        public async Task<Unit> Handle(ShowNpcCommand command, CancellationToken cancellationToken)
        {
            var playerId = command.PlayerId;
            var npcId = command.NpcId;

            var npcInfo = new NpcInfo()
            {
                Descriptions = new List<string>(),
                Actions = new List<NpcAction>(),
                Id = npcId
            };
            var npc = await _npcDomainService.Get(npcId);
            if (npc == null)
            {
                return Unit.Value;
            }

            npcInfo.Name = npc.Name;
            string genderStr = npc.Gender.ToGender();

            if (npc.CanChat)
            {
                npcInfo.Actions.Add(new NpcAction { Name = NpcActionEnum.闲聊.ToString() });
            }

            if (npc.Type == NpcTypeEnum.人物)
            {
                //npcInfo.Actions.Add(new NpcAction { Name = NpcActionEnum.给予.ToString() });
            }

            if (npc.CanFight)
            {
                npcInfo.Actions.Add(new NpcAction { Name = NpcActionEnum.切磋.ToString() });
            }

            if (npc.CanKill)
            {
                npcInfo.Actions.Add(new NpcAction { Name = NpcActionEnum.杀死.ToString() });
            }

            var player = await _playerDomainService.Get(playerId);

            npcInfo.Descriptions.Add(npc.Description ?? "");
            npcInfo.Descriptions.Add($"{genderStr}{npc.Age.ToAge()}");
            npcInfo.Descriptions.Add($"{genderStr}{npc.Per.ToPer(npc.Age, npc.Gender)}");
            npcInfo.Descriptions.Add($"{genderStr}{npc.Exp.ToKunFuLevel(player.Exp)}");


            var npcScripts = (await _npcScriptDomainService.GetAll()).Where(x => x.NpcId == npc.Id).ToList();
            foreach (var npcScript in npcScripts)
            {
                var script = await _scriptDomainService.Get(npcScript.ScriptId);

                if (script != null)
                {
                    npcInfo.Actions.Add(new NpcAction { Name = script.Name, ScriptId = script.Id, CommandId = 0 });
                }

            }


            await _mudProvider.ShowNpc(playerId, npcInfo);


         



            return Unit.Value;
        }



        public async Task<Unit> Handle(GiveToNpcCommand command, CancellationToken cancellationToken)
        {
            var playerId = command.PlayerId;
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                return Unit.Value;
            }

            var npcId = command.NpcId;
            var npc = await _npcDomainService.Get(npcId);
            if (npc == null)
            {
                return Unit.Value;
            }

            if (npc.Type != NpcTypeEnum.人物)
            {
                await _bus.RaiseEvent(new DomainNotification($"指令 错误！"));
            }
            else
            {
                await _bus.RaiseEvent(new DomainNotification($"指令 未实现！"));
            }

            return Unit.Value;
        }

        public async Task<Unit> Handle(KillNpcCommand command, CancellationToken cancellationToken)
        {
            var playerId = command.PlayerId;
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                return Unit.Value;
            }

            var npcId = command.NpcId;
            var npc = await _npcDomainService.Get(npcId);
            if (npc == null)
            {
                return Unit.Value;
            }

            if (!npc.CanKill)
            {
                await _bus.RaiseEvent(new DomainNotification($"指令 错误！"));
            }
            else
            {
                await _bus.RaiseEvent(new DomainNotification($"指令 未实现！"));
            }

            return Unit.Value;
        }

        public async Task<Unit> Handle(NpcScriptCommand command, CancellationToken cancellationToken)
        {
            var commandId = command.CommandId;
            var input = command.Message;
            var scriptId = command.ScriptId;

            var playerId = command.PlayerId;
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                return Unit.Value;
            }

            var npcId = command.NpcId;
            var npc = await _npcDomainService.Get(npcId);
            if (npc == null)
            {
                return Unit.Value;
            }


            await DoScript(player, npc, scriptId, commandId, input);


            return Unit.Value;
        }

        public async Task<Unit> Handle(ChatWithNpcCommand command, CancellationToken cancellationToken)
        {
            var playerId = command.PlayerId;
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                return Unit.Value;
            }

            var npcId = command.NpcId;
            var npc = await _npcDomainService.Get(npcId);
            if (npc == null)
            {
                return Unit.Value;
            }

            if (npc.Type != NpcTypeEnum.人物)
            {
                await _bus.RaiseEvent(new DomainNotification($"指令 错误！"));
                return Unit.Value;
            }


            await _mudProvider.ShowMessage(player.Id, $"与 [{npc.Name}] 闲聊中...");

            var chatWithNpcLike = await _redisDb.StringGet<int>(string.Format(RedisKey.ChatWithNpcLike, playerId, npcId));
            if (chatWithNpcLike > 0)
            {
                return Unit.Value;
            }

            Random random = new Random();

            int kar = Math.Abs(npc.Kar - player.Kar);

            if (random.Next(1, 100) > kar)
            {
                var npcRelation = await _npcRelationDomainService.Get(x => x.PlayerId == player.Id && x.NpcId == npcId);
                if (npcRelation == null)
                {
                    npcRelation = new NpcRelationEntity
                    {
                        CreatedTime = DateTime.Now,
                        NpcId = npcId,
                        Liking = 1,
                        PlayerId = player.Id
                    };
                    await _npcRelationDomainService.Add(npcRelation);
                }
                else
                {
                    if (npcRelation.Liking < 20)
                    {
                        npcRelation.Liking++;
                        await _npcRelationDomainService.Update(npcRelation);
                    }
                }

                await _mudProvider.ShowMessage(player.Id, $"交谈甚欢，与[{npc.Name}]的好感度上升");
            }

            await _bus.RaiseEvent(new ChatWithNpcEvent(playerId, npc.Id)).ConfigureAwait(false);

            return Unit.Value;
        }
        #region NpcScript


        private async Task DoScript(PlayerEntity player, NpcEntity npc, int scriptId, int commandId, string input = "")
        {
            _logger.LogDebug($"npc={npc.Id},scriptId={scriptId},commandId={commandId},input={input}");

            var npcScripts = (await _npcScriptDomainService.GetAll()).Where(x => x.NpcId == npc.Id);
            var scriptIds = npcScripts.Select(x => x.ScriptId).ToList();
            if (!scriptIds.Contains(scriptId))
            {
                //await _bus.RaiseEvent(new DomainNotification($"脚本不存在！"));
                return;
            }

            var script = await _scriptDomainService.Get(scriptId);
            if (script == null)
            {
                //await _bus.RaiseEvent(new DomainNotification($"脚本不存在！"));
                return;
            }
            ScriptCommandEntity scriptCommand;
            var scriptCommands = await _scriptCommandDomainService.Query(x => x.ScriptId == scriptId);
            if (commandId == 0)
            {
                scriptCommand = scriptCommands.FirstOrDefault(x => x.IsEntry);
            }
            else
            {
                scriptCommand = scriptCommands.FirstOrDefault(x => x.Id == commandId);
            }

            if (scriptCommand == null)
            {
                return;
            }

            if (!scriptCommand.IsEntry)
            {
                var key = string.Format(RedisKey.CommandIds, player.Id, npc.Id, scriptId);
                var commandIds = await _redisDb.StringGet<List<int>>(key);
                if (commandIds == null || !commandIds.Contains(scriptCommand.Id))
                {
                    return;
                }
            }

            var checkIf = true;//判断if条件是否为true

            var caseIfStr = scriptCommand.CaseIf;


            if (!string.IsNullOrEmpty(caseIfStr))
            {
                List<CaseIf> caseIfs = new List<CaseIf>();
                try
                {
                    caseIfs = JsonConvert.DeserializeObject<List<CaseIf>>(caseIfStr);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Convert CaseIf:{ex}");
                }

                foreach (var caseIf in caseIfs)
                {
                    checkIf = await CheckIf(player, caseIf.Condition, caseIf.Attrs, input);
                    if (!checkIf)
                    {
                        break;
                    }
                }
            }


            if (checkIf)
            {
                //执行then分支
                var caseThenStr = scriptCommand.CaseThen;


                if (!string.IsNullOrEmpty(caseThenStr))
                {
                    List<CaseThen> caseThens = new List<CaseThen>();
                    try
                    {
                        caseThens = JsonConvert.DeserializeObject<List<CaseThen>>(caseThenStr);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Convert CaseThen:{ex}");
                    }


                    foreach (var caseThen in caseThens)
                    {
                        await DoCommand(player, npc, scriptId, caseThen.Command, caseThen.Attrs, input);
                    }
                }
            }
            else
            {
                //执行else分支
                var caseElseStr = scriptCommand.CaseElse;
                if (!string.IsNullOrEmpty(caseElseStr))
                {
                    List<CaseElse> caseElses = new List<CaseElse>();
                    try
                    {
                        caseElses = JsonConvert.DeserializeObject<List<CaseElse>>(caseElseStr);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Convert CaseElse:{ex}");
                    }

                    foreach (var caseElse in caseElses)
                    {
                        await DoCommand(player, npc, scriptId, caseElse.Command, caseElse.Attrs, input);
                    }
                }
            }
        }

        private async Task<bool> CheckIf(PlayerEntity player, string condition, List<CaseAttribute> attrs, string input)
        {
            var field = attrs.FirstOrDefault(x => x.Attr == "Field")?.Val;
            var relation = attrs.FirstOrDefault(x => x.Attr == "Relation")?.Val;
            var value = attrs.FirstOrDefault(x => x.Attr == "Value")?.Val;
            var wareName = attrs.FirstOrDefault(x => x.Attr == "WareName")?.Val;
            var number = attrs.FirstOrDefault(x => x.Attr == "Number")?.Val;
            int.TryParse(attrs.FirstOrDefault(x => x.Attr == "QuestId")?.Val, out int questId);

            if (string.IsNullOrEmpty(condition))
            {
                return true;
            }

            if (!Enum.TryParse(condition, true, out ConditionTypeEnum conditionEnum))
            {
                return true;
            }


            switch (conditionEnum)
            {
                case ConditionTypeEnum.角色属性:
                    if (!CheckField(player, field, value, relation))
                    {
                        return false;
                    }
                    break;

                case ConditionTypeEnum.是否拥有物品:
                    if (!await CheckWare(player.Id, wareName, number, relation))
                    {
                        return false;
                    }

                    break;

                case ConditionTypeEnum.是否领取任务:

                    var playerQuest = await _playerQuestDomainService.Get(x => x.QuestId == questId && x.PlayerId == player.Id);
                    if (playerQuest == null)
                    {
                        return false;
                    }
                    break;

                case ConditionTypeEnum.是否完成任务:

                    break;

                case ConditionTypeEnum.活动记录:

                    break;
            }

            return true;
        }

        private async Task DoCommand(PlayerEntity player, NpcEntity npc, int scriptId, string command, List<CaseAttribute> attrs, string input)
        {
            var title = attrs.FirstOrDefault(x => x.Attr == "Title")?.Val;
            var message = attrs.FirstOrDefault(x => x.Attr == "Message")?.Val;
            var tips = attrs.FirstOrDefault(x => x.Attr == "Tips")?.Val;
            int.TryParse(attrs.FirstOrDefault(x => x.Attr == "CommandId")?.Val, out int commandId);
            int.TryParse(attrs.FirstOrDefault(x => x.Attr == "QuestId")?.Val, out int questId);



            var key = $"commandIds_{player.Id}_{npc.Id}_{scriptId}";
            var commandIds = await _redisDb.StringGet<List<int>>(key) ?? new List<int>();
            if (commandId > 0 && !commandIds.Contains(commandId))
            {
                commandIds.Add(commandId);
            }

            await _redisDb.StringSet(key, commandIds);

            var commandEnum = (CommandTypeEnum)Enum.Parse(typeof(CommandTypeEnum), command, true);

            //await _bus.RaiseEvent(new DomainNotification($"command= {command}"));
            switch (commandEnum)
            {
                case CommandTypeEnum.播放对话:
                    await _mudProvider.ShowMessage(player.Id, $"{npc.Name}：{message}", MessageTypeEnum.聊天);
                    break;

                case CommandTypeEnum.对话选项:
                    await _mudProvider.ShowMessage(player.Id, $" → <a href='javascript:;' class='chat' npcId='{npc.Id}' scriptId='{scriptId}' commandId='{commandId}'>{title}</a><br />", MessageTypeEnum.指令);
                    break;

                case CommandTypeEnum.输入选项:
                    await _mudProvider.ShowMessage(player.Id, $" → <a href = 'javascript:;'>{tips}</a>  <input type = 'text' name='input' style='width:120px;margin-left:10px;' />  <button type = 'button' class='input' style='padding:1px 3px;' npcId='{npc.Id}'  scriptId='{scriptId}'  commandId='{commandId}'> 确定 </button><br />", MessageTypeEnum.指令);
                    break;

                case CommandTypeEnum.跳转到分支:
                    await DoScript(player, npc, scriptId, commandId);
                    break;


                case CommandTypeEnum.领取任务:
                    await TakeQuest(player, questId);
                    break;

                case CommandTypeEnum.完成任务:
                    await ComplateQuest(player, questId);
                    break;
            }
        }


        private async Task ComplateQuest(PlayerEntity player, int questId)
        {

            var quest = await _questDomainService.Get(questId);
            if (quest == null)
            {
                return;
            }

            var playerQuest = await _playerQuestDomainService.Get(x => x.PlayerId == player.Id && x.QuestId == questId);
            if (playerQuest == null)
            {
                return;
            }

            //未领取
            if (playerQuest.Status != QuestStateEnum.已领取进行中)
            {
                return;
            }

            //TODO 修改任务状态
            //playerQuest.HasTake = false;
            playerQuest.CompleteDate = DateTime.Now;
            playerQuest.Status = QuestStateEnum.完成已领奖;
            playerQuest.CompleteTimes++;
            //playerQuest.IsComplete = true;
            await _playerQuestDomainService.Update(playerQuest);

            await _mudProvider.ShowMessage(player.Id, $"你完成了任务 [{quest.Name}]");

            await TakeQuestReward(player, quest.Reward);

            await _bus.RaiseEvent(new CompleteQuestEvent(player, quest)).ConfigureAwait(false);

        }


        private async Task TakeQuest(PlayerEntity player, int questId)
        {
            var questToTake = await _questDomainService.Get(questId);
            if (questToTake == null)
            {
                return;
            }

            var playerQuestToTake = await _playerQuestDomainService.Get(x => x.PlayerId == player.Id && x.QuestId == questToTake.Id);

            if (playerQuestToTake == null)
            {
                playerQuestToTake = new PlayerQuestEntity
                {
                    PlayerId = player.Id,
                    QuestId = questId,
                    Status = QuestStateEnum.已领取进行中,
                    //IsComplete = false,
                    TakeDate = DateTime.Now,
                    CompleteDate = DateTime.Now,
                    CreateDate = DateTime.Now,
                    DayTimes = 1,
                    //HasTake = true,
                    Target = questToTake.Target,
                    Times = 1,
                    UpdateDate = DateTime.Now
                };
                await _playerQuestDomainService.Add(playerQuestToTake);

            }
            else if (playerQuestToTake.Status == QuestStateEnum.完成已领奖 || playerQuestToTake.Status == QuestStateEnum.未领取)
            {
                //TODO 领取任务
                //playerQuestToTake.HasTake = true;
                //playerQuestToTake.IsComplete = false;

                playerQuestToTake.Status = QuestStateEnum.已领取进行中;
                playerQuestToTake.TakeDate = DateTime.Now;
                playerQuestToTake.Times += 1;
                playerQuestToTake.Target = questToTake.Target;

                await _playerQuestDomainService.Update(playerQuestToTake);
            }
        }

        private async Task<bool> CheckWare(int playerId, string wareName, string number, string strRelation)
        {
            if (!int.TryParse(number, out int numberValue))
            {
                return false;
            }

            var ware = await _wareDomainService.Get(x => x.Name == wareName);
            if (ware == null)
            {
                return false;
            }

            var playerWare = await _playerWareDomainService.Get(x => x.WareId == ware.Id && x.PlayerId == playerId);
            if (playerWare == null)
            {
                return false;
            }

            var relations = GetRelations(strRelation);
            return relations.Contains(playerWare.Number.CompareTo(numberValue));
        }

        private List<int> GetRelations(string relation)
        {
            List<int> relations = new List<int>();// 1 大于，0 等于 ，-1 小于
            var relationEnum = (LogicalRelationTypeEnum)Enum.Parse(typeof(LogicalRelationTypeEnum), relation, true);
            switch (relationEnum)
            {
                case LogicalRelationTypeEnum.不等于:
                    relations = new List<int>() { 1, -1 };
                    break;

                case LogicalRelationTypeEnum.大于:
                    relations = new List<int>() { 1 };
                    break;

                case LogicalRelationTypeEnum.大于等于:
                    relations = new List<int>() { 1, 0 };
                    break;

                case LogicalRelationTypeEnum.小于:
                    relations = new List<int>() { -1 };
                    break;

                case LogicalRelationTypeEnum.小于等于:
                    relations = new List<int>() { 0, -1 };
                    break;

                case LogicalRelationTypeEnum.等于:
                    relations = new List<int>() { 0 };
                    break;
            }

            return relations;
        }

        private bool CheckField(PlayerEntity player, string field, string strValue, string strRelation)
        {
            try
            {
                var relations = GetRelations(strRelation);// 1 大于，0 等于 ，-1 小于

                var fieldProp = GetFieldPropertyInfo(player, field);
                if (fieldProp == null)
                {
                    return false;
                }

                var objectValue = fieldProp.GetValue(player);
                var typeCode = Type.GetTypeCode(fieldProp.GetType());
                switch (typeCode)
                {
                    case TypeCode.Int32:
                        return relations.Contains(Convert.ToInt32(strValue).CompareTo(Convert.ToInt32(objectValue)));

                    case TypeCode.Int64:
                        return relations.Contains(Convert.ToInt64(strValue).CompareTo(Convert.ToInt64(objectValue)));

                    case TypeCode.Decimal:
                        return relations.Contains(Convert.ToDecimal(strValue).CompareTo(Convert.ToDecimal(objectValue)));

                    case TypeCode.Double:
                        return relations.Contains(Convert.ToDouble(strValue).CompareTo(Convert.ToDouble(objectValue)));

                    case TypeCode.Boolean:
                        return relations.Contains(Convert.ToBoolean(strValue).CompareTo(Convert.ToBoolean(objectValue)));

                    case TypeCode.DateTime:
                        return relations.Contains(Convert.ToDateTime(strValue).CompareTo(Convert.ToDateTime(objectValue)));

                    case TypeCode.String:
                        return relations.Contains(strValue.CompareTo(objectValue));

                    default:
                        throw new Exception($"不支持的数据类型： {typeCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"CheckField Exception:{ex}");
                return false;
            }
        }

        private PropertyInfo GetFieldPropertyInfo(PlayerEntity player, string field)
        {
            var fieldEnum = (PlayerConditionFieldEnum)Enum.Parse(typeof(PlayerConditionFieldEnum), field, true);


            var key = $"player_properties";
            var properties = _cache.GetOrCreate(key, p => {
                p.SetAbsoluteExpiration(TimeSpan.FromHours(24));
                return player.GetType().GetProperties();
            });

            foreach (var prop in properties)
            {
                var attribute = prop.GetCustomAttributes(typeof(ConditionFieldAttribute), true).FirstOrDefault();
                if (attribute != null)
                {
                    if ((attribute as ConditionFieldAttribute).FieldEnum == fieldEnum)
                    {
                        return prop;
                    }
                }
            }

            return null;
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

        #endregion


    }
}
