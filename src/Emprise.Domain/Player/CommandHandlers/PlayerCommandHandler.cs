using AutoMapper;
using Emprise.Domain.Common.Modes;
using Emprise.Domain.Core.Attributes;
using Emprise.Domain.Core.Authorization;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Bus.Models;
using Emprise.Domain.Core.CommandHandlers;
using Emprise.Domain.Core.Enum;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Notifications;
using Emprise.Domain.Npc.Entity;
using Emprise.Domain.Npc.Models;
using Emprise.Domain.Npc.Services;
using Emprise.Domain.Player.Commands;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.Player.Events;
using Emprise.Domain.Player.Services;
using Emprise.Domain.Quest.Entity;
using Emprise.Domain.Quest.Models;
using Emprise.Domain.Quest.Services;
using Emprise.Domain.Room.Services;
using Emprise.Domain.Ware.Services;
using Emprise.Infra;
using Emprise.Infra.Authorization;
using Emprise.Infra.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emprise.Domain.User.CommandHandlers
{
    
    public class PlayerCommandHandler : CommandHandler, 
        IRequestHandler<CreateCommand, Unit>,
        IRequestHandler<JoinGameCommand, Unit>,
        IRequestHandler<InitGameCommand, Unit>,
        IRequestHandler<MoveCommand, Unit>,
        IRequestHandler<SearchCommand, Unit>,
        IRequestHandler<MeditateCommand, Unit>,
        IRequestHandler<StopActionCommand, Unit>,
        IRequestHandler<ExertCommand, Unit>,
        IRequestHandler<NpcActionCommand, Unit>,
        IRequestHandler<QuestCommand, Unit>,
        IRequestHandler<CompleteQuestCommand, Unit>

        

    {
        private readonly IMediatorHandler _bus;
        private readonly ILogger<PlayerCommandHandler> _logger;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly IMapper _mapper;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly IRoomDomainService _roomDomainService;
        private readonly INpcDomainService _npcDomainService;
        private readonly IAccountContext _account;
        private readonly IDelayedQueue  _delayedQueue;
        private readonly IRecurringQueue _recurringQueue;
        private readonly IMudProvider _mudProvider;
        private readonly AppConfig _appConfig;
        private readonly IScriptDomainService _scriptDomainService;
        private readonly INpcScriptDomainService _npcScriptDomainService;
        private readonly IScriptCommandDomainService _ScriptCommandDomainService;
        private readonly IWareDomainService _wareDomainService;
        private readonly IPlayerWareDomainService _playerWareDomainService;
        private readonly IQuestDomainService _questDomainService ;
        private readonly IPlayerQuestDomainService _playerQuestDomainService;
        private readonly IRedisDb _redisDb;
        private readonly IMemoryCache _cache;

        public PlayerCommandHandler(
            IMediatorHandler bus,
            ILogger<PlayerCommandHandler> logger,
            IHttpContextAccessor httpAccessor,
            IMapper mapper,
            IPlayerDomainService playerDomainService,
            IRoomDomainService roomDomainService,
            INpcDomainService npcDomainService,
            IAccountContext account,
            IDelayedQueue delayedQueue,
            IRecurringQueue recurringQueue,
            IMudProvider mudProvider,
            IOptions<AppConfig> appConfig,
            IScriptDomainService scriptDomainService,
            INpcScriptDomainService npcScriptDomainService,
            IScriptCommandDomainService ScriptCommandDomainService,
            IWareDomainService wareDomainService,
            IPlayerWareDomainService playerWareDomainService,
            IQuestDomainService questDomainService,
            IPlayerQuestDomainService playerQuestDomainService,
            IRedisDb redisDb,
            IMemoryCache cache,
            INotificationHandler<DomainNotification> notifications) : base(bus, notifications)
        {

            _cache = cache;
            _bus = bus;
            _logger = logger;
            _httpAccessor = httpAccessor;
            _mapper = mapper;
            _playerDomainService = playerDomainService;
            _roomDomainService = roomDomainService;
            _account = account;
            _delayedQueue = delayedQueue;
            _recurringQueue = recurringQueue;
            _mudProvider = mudProvider;
            _appConfig = appConfig.Value;
            _scriptDomainService = scriptDomainService;
            _npcScriptDomainService = npcScriptDomainService;
            _ScriptCommandDomainService = ScriptCommandDomainService;
            _npcDomainService = npcDomainService;
            _wareDomainService = wareDomainService;
            _playerWareDomainService = playerWareDomainService;
            _questDomainService = questDomainService;
            _playerQuestDomainService = playerQuestDomainService;
            _redisDb = redisDb;
        }

        public async Task<Unit> Handle(CreateCommand command, CancellationToken cancellationToken)
        {
            var name = command.Name;
            var gender = command.Gender;
            var userId = command.UserId;
            var str = command.Str;
            var @int = command.Int;
            var dex = command.Dex;
            var con = command.Con;

            var player = await _playerDomainService.Get(p => p.Name == name);

            if (player != null)
            {
                await _bus.RaiseEvent(new DomainNotification("角色名已被使用，请更改！"));
                return Unit.Value;
            }

            player = await _playerDomainService.GetUserPlayer(userId);
            if (player != null)
            {
                await _bus.RaiseEvent(new DomainNotification("已经超过最大可创建角色数！"));
                return Unit.Value;
            }

            if (str + @int + dex + con != 80)
            {
                await _bus.RaiseEvent(new DomainNotification("所有先天属性之和必须为80！"));
                return Unit.Value;
            }

            var roomId = _appConfig.Site.BornRoomId;
            if (roomId <= 0)
            {
                await _bus.RaiseEvent(new DomainNotification("未设置出生地点！"));
                return Unit.Value;
            }

            var room = await _roomDomainService.Get(roomId);
            if (room == null)
            {
                await _bus.RaiseEvent(new DomainNotification("设置的出生地点不存在！"));
                return Unit.Value;
            }

            player = new PlayerEntity
            {
                CreateDate = DateTime.Now,
                LastDate = DateTime.Now,
                Level = 1,
                Name = name,
                UserId = userId,
                Status = PlayerStatusEnum.空闲,
                Gender = gender,
                Age = 14 * 12,
                ConAdd = 0,
                DexAdd = 0,
                FactionId = 0,
                IntAdd = 0,
                Money = 0,
                RoomId = roomId,
                Title = "",
                StrAdd = 0,
                Atk = str,
                Str = str,
                Con = con,
                Int = @int,
                Dex = dex,
                Exp = 0,
                Cor = 20,
                Cps = 20,
                Flee = 0,
                Hit = 0,
                Pot = 0,
                Kar = 20,
                Def = 0,
                Hp = 0,
                LimitMp = 0,
                MaxHp = 0,
                MaxMp = 0,
                Mp = 0,
                Parry = 0,
                Per = 0,
                Nrg = 0
            };


            await _playerDomainService.Add(player);

            var jwtAccount = new JwtAccount
            {
                UserId = userId,
                Email = _account.Email,
                PlayerId = player.Id,
                PlayerName = player.Name
            };

            await _httpAccessor.HttpContext.SignIn(CookieAuthenticationDefaults.AuthenticationScheme, jwtAccount);

            await _bus.RaiseEvent(new CreatedEvent(player)).ConfigureAwait(false);

            return Unit.Value;
        }

        public async Task<Unit> Handle(JoinGameCommand command, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Handle JoinGameCommand:{JsonConvert.SerializeObject(command)}");
            var userId = command.UserId;
            var playerId = command.PlayerId;

            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            if (player.UserId != userId)
            {
                await _bus.RaiseEvent(new DomainNotification("该角色不存在！"));
                return Unit.Value;
            }

            if (_account == null)
            {
                await _bus.RaiseEvent(new DomainNotification("未登录！"));
                return Unit.Value;
            }

            var jwtAccount = new JwtAccount
            {
                UserId = userId,
                Email = _account.Email,
                PlayerId = playerId,
                PlayerName = player.Name
            };

            await _httpAccessor.HttpContext.SignIn(CookieAuthenticationDefaults.AuthenticationScheme, jwtAccount);

            await _bus.RaiseEvent(new JoinedGameEvent(player)).ConfigureAwait(false);

            return Unit.Value;
        }

        public async Task<Unit> Handle(InitGameCommand command, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Handle InitGameCommand:{JsonConvert.SerializeObject(command)}");

            var playerId = command.PlayerId;

            if (playerId <= 0)
            {
                await _bus.RaiseEvent(new DomainNotification($"请重新进入！"));
                return Unit.Value;
            }

            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            var room = await _roomDomainService.Get(player.RoomId);
            if (room == null)
            {
                await _bus.RaiseEvent(new DomainNotification("场景不存在！"));
                return Unit.Value;
            }
            await _bus.RaiseEvent(new InitGameEvent(player)).ConfigureAwait(false);
            await _bus.RaiseEvent(new PlayerInRoomEvent(player, room)).ConfigureAwait(false);
        

            return Unit.Value;
        }

        public async Task<Unit> Handle(MoveCommand command, CancellationToken cancellationToken)
        {

            var roomId = command.RoomId;
            var playerId = command.PlayerId;
           
            if (roomId <= 0)
            {
                await _bus.RaiseEvent(new DomainNotification($"场景不存在！"));
                return Unit.Value;
            }

            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }
            if (player.Status!= PlayerStatusEnum.空闲)
            {
                await _bus.RaiseEvent(new DomainNotification($"请先停止{player.Status}！"));
                return Unit.Value;
            }
            var oldRoomId = player.RoomId;
            if (oldRoomId == roomId)
            {
                await _bus.RaiseEvent(new DomainNotification("你已在此地！请刷新或重新进入游戏！"));
                return Unit.Value;
            }
            var oldRoom = await _roomDomainService.Get(oldRoomId);
            if (oldRoom != null)
            {
                if (!new[] { oldRoom.West, oldRoom.East, oldRoom.North, oldRoom.South }.Contains(roomId))
                {
                    await _bus.RaiseEvent(new DomainNotification($"无法移动到该场景！"));
                    return Unit.Value;
                }
            }

            var newRoom = await _roomDomainService.Get(roomId);
            if (newRoom == null)
            {
                await _bus.RaiseEvent(new DomainNotification("场景不存在！"));
                return Unit.Value;
            }

            player.RoomId = roomId;
            await _playerDomainService.Update(player);

            /*
            var jwtAccount = new JwtAccount
            {
                UserId = _account.UserId,
                Email = _account.Email,
                PlayerId = _account.PlayerId,
                PlayerName = _account.PlayerName
            };

            await _httpAccessor.HttpContext.SignIn(CookieAuthenticationDefaults.AuthenticationScheme, jwtAccount);
            */

            //await _bus.RaiseEvent(new MovedEvent(player)).ConfigureAwait(false);

            await _bus.RaiseEvent(new MovedEvent(player, newRoom, oldRoom));
            

            return Unit.Value;
        }

        public async Task<Unit> Handle(SearchCommand command, CancellationToken cancellationToken)
        {
            var playerId = command.PlayerId;
            //await _bus.RaiseEvent(new DomainNotification("你什么也没发现"));
            await _mudProvider.ShowMessage(playerId, "你什么也没发现。。。");
            return Unit.Value;
        }

        public async Task<Unit> Handle(MeditateCommand command, CancellationToken cancellationToken)
        {
            //await _bus.RaiseEvent(new DomainNotification("功能暂时未实现"));

            var playerId = command.PlayerId;
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            if (player.Status == PlayerStatusEnum.打坐)
            {
                return Unit.Value;
            }

            if (player.Status != PlayerStatusEnum.空闲)
            {
                await _bus.RaiseEvent(new DomainNotification($"你正在{player.Status}中，请先停下！"));
                return Unit.Value;
            }

            await _mudProvider.ShowMessage(playerId, "开始打坐。。。");
            player.Status = PlayerStatusEnum.打坐;
            await _playerDomainService.Update(player);

            await _recurringQueue.Publish(playerId, new MeditateModel { }, 2,10);

            await _bus.RaiseEvent(new PlayerStatusChangedEvent(player)).ConfigureAwait(false);

            return Unit.Value;
        }

        public async Task<Unit> Handle(StopActionCommand command, CancellationToken cancellationToken)
        {
            //await _bus.RaiseEvent(new DomainNotification("功能暂时未实现"));

            var playerId = command.PlayerId;
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }
            var status = player.Status;
            if (status == PlayerStatusEnum.空闲)
            {
                return Unit.Value;
            }


            player.Status = PlayerStatusEnum.空闲;
            await _playerDomainService.Update(player);

            switch (status)
            {
                case PlayerStatusEnum.打坐:
                    await _mudProvider.ShowMessage(playerId, "你停止了打坐。。。");
                    await _recurringQueue.Remove<MeditateModel>(playerId);
                    break;

                case PlayerStatusEnum.工作:
                    await _mudProvider.ShowMessage(playerId, "你停止了工作。。。");
                    await _recurringQueue.Remove<MeditateModel>(playerId);
                    break;
            }
          

            await _bus.RaiseEvent(new PlayerStatusChangedEvent(player)).ConfigureAwait(false);

            return Unit.Value;
        }

        public async Task<Unit> Handle(ExertCommand command, CancellationToken cancellationToken)
        {
            //await _bus.RaiseEvent(new DomainNotification("功能暂时未实现"));

            var playerId = command.PlayerId;
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }
            var status = player.Status;
            if (status == PlayerStatusEnum.疗伤)
            {
                return Unit.Value;
            }

            if (player.Status != PlayerStatusEnum.空闲)
            {
                await _bus.RaiseEvent(new DomainNotification($"你正在{player.Status}中，请先停下！"));
                return Unit.Value;
            }

            await _mudProvider.ShowMessage(playerId, "你盘膝坐下，开始运功疗伤。");
            player.Status = PlayerStatusEnum.疗伤;
            await _playerDomainService.Update(player);

            await _recurringQueue.Publish(playerId, new ExertModel {  }, 2, 10);

            await _bus.RaiseEvent(new PlayerStatusChangedEvent(player)).ConfigureAwait(false);


            return Unit.Value;
        }

        public async Task<Unit> Handle(NpcActionCommand command, CancellationToken cancellationToken)
        {

            var npcId = command.NpcId;
            var playerId = command.PlayerId;
            var commandId = command.CommandId;
            var commandName = command.CommandName;
            var input = command.Message;
            var scriptId = command.ScriptId;
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            var npc = await _npcDomainService.Get(npcId);
            if (npc == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"npc不存在！"));
                return Unit.Value;
            }

            if (scriptId > 0 && commandId > 0)
            {
                await DoScript(player, npc, scriptId, commandId, input);
            }
            else
            {
                await DoAction(player, npc, commandName);
            }

         
            return Unit.Value;
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

            var playerQuest = await _playerQuestDomainService.Get(x => x.PlayerId == playerId && x.QuestId == quest.Id);
            if (playerQuest != null)
            {
                //已领取
                if (playerQuest.HasTake)
                {
                    await _mudProvider.ShowMessage(playerId, quest.InProgressWords);
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
                    HasComplete = false,
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
                playerQuest.HasComplete = false;
                playerQuest.IsComplete = false;
                playerQuest.TakeDate = DateTime.Now;
                playerQuest.Times += 1;
                playerQuest.Target = quest.Target;

                await _playerQuestDomainService.Update(playerQuest);
            }


          

            await _bus.RaiseEvent(new DomainNotification($"领取任务 {quest.Name} ！"));

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
            if (!playerQuest.HasTake)
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
            playerQuest.HasTake = false;
            playerQuest.CompleteDate = DateTime.Now;
            playerQuest.HasComplete = true;
            await _playerQuestDomainService.Update(playerQuest);

            await DoQuestConsume(player, quest.Consume);


            await TakeQuestReward(player, quest.Reward);
            //TODO  领取奖励

            await _mudProvider.ShowMessage(player.Id, quest.CompletedWords);

            return Unit.Value;
        }



        
        #region 私有方法



        private async Task DoAction(PlayerEntity player, NpcEntity npc,string commandName)
        {
            NpcActionEnum actionEnum;
            if (Enum.TryParse(commandName, out actionEnum))
            {
                switch (actionEnum)
                {
                    case NpcActionEnum.切磋:
                        if (!npc.CanFight)
                        {
                            await _bus.RaiseEvent(new DomainNotification($"指令 {commandName} 错误！"));
                        }
                        else
                        {
                            await _bus.RaiseEvent(new DomainNotification($"指令 {commandName} 未实现！"));
                        }
                        break;

                    case NpcActionEnum.杀死:
                        if (!npc.CanKill)
                        {
                            await _bus.RaiseEvent(new DomainNotification($"指令 {commandName} 错误！"));
                        }
                        else
                        {
                            await _bus.RaiseEvent(new DomainNotification($"指令 {commandName} 未实现！"));
                        }
                        break;

                    case NpcActionEnum.给予:
                        if (npc.Type != NpcTypeEnum.人物)
                        {
                            await _bus.RaiseEvent(new DomainNotification($"指令 {commandName} 错误！"));
                        }
                        else
                        {
                            await _bus.RaiseEvent(new DomainNotification($"指令 {commandName} 未实现！"));
                        }
                        break;
                }
            }
        }

        private async Task DoScript(PlayerEntity player, NpcEntity npc, int scriptId, int commandId, string input = "")
        {
            _logger.LogDebug($"npc={npc.Id},scriptId={scriptId},commandId={commandId},input={input}");

            var npcScripts = await _npcScriptDomainService.Query(x => x.NpcId == npc.Id);
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

            var scriptCommands = await _ScriptCommandDomainService.Query(x => x.ScriptId == scriptId);

            var scriptCommand = scriptCommands.FirstOrDefault(x => x.Id == commandId);
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
                catch(Exception ex)
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


            var conditionEnum = (ConditionTypeEnum)Enum.Parse(typeof(ConditionTypeEnum), condition, true);
            

            switch (conditionEnum)
            {
                case ConditionTypeEnum.角色属性:
                    if (!CheckField(player, field, value, relation))
                    {
                        return false;
                    }
                    break;

                case ConditionTypeEnum.拥有物品:
                    if (!await CheckWare(player.Id, wareName, number, relation))
                    {
                        return false;
                    }

                    break;

                case ConditionTypeEnum.完成任务:

                    break;

                case ConditionTypeEnum.活动记录:

                    break;
            }

            return true;
        }

        private async Task DoCommand(PlayerEntity player, NpcEntity npc, int scriptId, string command,List<CaseAttribute> attrs, string input)
        {
            var title = attrs.FirstOrDefault(x => x.Attr == "Title")?.Val;
            var message = attrs.FirstOrDefault(x => x.Attr == "Message")?.Val;
            var tips = attrs.FirstOrDefault(x => x.Attr == "Tips")?.Val;
            int.TryParse(attrs.FirstOrDefault(x => x.Attr == "CommandId")?.Val, out int commandId);
            int.TryParse(attrs.FirstOrDefault(x => x.Attr == "QuestId")?.Val, out int questId);



            var key = $"commandIds_{player.Id}_{npc.Id}_{scriptId}";
            var commandIds = await _redisDb.StringGet<List<int>>(key) ?? new List<int>();
            commandIds.Add(commandId);
            await _redisDb.StringSet(key, commandIds);

            var commandEnum = (CommandTypeEnum)Enum.Parse(typeof(CommandTypeEnum), command, true);

            //await _bus.RaiseEvent(new DomainNotification($"command= {command}"));
            switch (commandEnum)
            {
                case CommandTypeEnum.播放对话:

                    await _mudProvider.ShowMessage(player.Id, message);

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

                case CommandTypeEnum.触发任务:


                    var quest = await _questDomainService.Get(questId);
                    if (quest == null)
                    {
                        return;
                    }


                    bool canTake = false;

                    var playerQuest = await _playerQuestDomainService.Get(x => x.PlayerId == player.Id && x.QuestId == quest.Id);
                    if (playerQuest == null)
                    {
                        canTake = true;
                    }
                    else
                    {
                        if (playerQuest.HasTake)
                        {

                            var checkResult = await CheckQuestIfComplete(player, playerQuest.Target);
                            if (!checkResult.IsSuccess)
                            {
                                //任务未完成
                                await _mudProvider.ShowMessage(player.Id, quest.InProgressWords);
                                return;
                            }


                            await _mudProvider.ShowMessage(player.Id, quest.CompletedWords);

                            await _mudProvider.ShowMessage(player.Id, $"你已经完成任务 [{quest.Name}]。  <button type = 'button' class='completeQuest' style='padding:1px 3px;' questId='{quest.Id}'> 领取奖励 </button> ", MessageTypeEnum.指令);
                            return;
                        }

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


                    if (canTake)
                    {
                        await _mudProvider.ShowMessage(player.Id, quest.BeforeCreate);

                        await _mudProvider.ShowMessage(player.Id, $"是否领取该任务 [{quest.Name}]?  <button type = 'button' class='quest' style='padding:1px 3px;' questId='{quest.Id}'> 确定 </button> ", MessageTypeEnum.指令);
                    }
                    else
                    {
                        await _mudProvider.ShowMessage(player.Id, quest.CompletedWords);
                    }

                    break;
            }
        }

        private async Task<bool> CheckWare(int playerId, string wareName, string number, string strRelation)
        {
            if(!int.TryParse(number, out int numberValue))
            {
                return false;
            }

            var ware = await _wareDomainService.Get(x=>x.Name== wareName);
            if (ware==null)
            {
                return false;
            }

            var playerWare = await _playerWareDomainService.Get(x=>x.WareId== ware.Id && x.PlayerId== playerId);
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
                if (fieldProp==null)
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
        /// 判断用户是否完成某个任务
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
                    var npcId = questReward.Attrs.FirstOrDefault(x => x.Attr == "NpcId")?.Val;
                    int.TryParse(questReward.Attrs.FirstOrDefault(x => x.Attr == "Exp")?.Val, out int exp);
                    long.TryParse(questReward.Attrs.FirstOrDefault(x => x.Attr == "Money")?.Val, out long money);

                    var rewardEnum = (QuestRewardEnum)Enum.Parse(typeof(QuestRewardEnum), questReward.Reward, true);


                    switch (rewardEnum)
                    {


                        case QuestRewardEnum.物品:

                            //TODO 添加物品
                            result.ErrorMessage = $"完成任务需要消耗物品";
                            return result;
                            break;

                        case QuestRewardEnum.经验:
                            player.Exp += exp;
                            break;

                        case QuestRewardEnum.金钱:
                            player.Money += money;

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
