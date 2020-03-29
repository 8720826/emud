using AutoMapper;
using Emprise.Domain.Core.Authorization;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Bus.Models;
using Emprise.Domain.Core.CommandHandlers;
using Emprise.Domain.Core.Enum;
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
using Emprise.Domain.Room.Services;
using Emprise.Domain.Ware.Services;
using Emprise.Infra.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
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
        IRequestHandler<NpcActionCommand, Unit>


        
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
        private readonly IRedisDb _redisDb;

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
            IRedisDb redisDb,
            INotificationHandler<DomainNotification> notifications) : base(bus, notifications)
        {
           

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
                await DoScript(player, npc, scriptId, commandId);
            }
            else
            {
                await DoAction(player, npc, commandName);
            }
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

        private async Task DoScript(PlayerEntity player, NpcEntity npc, int scriptId, int commandId)
        {
            var npcScripts = await _npcScriptDomainService.Query(x => x.NpcId == npc.Id);
            var scriptIds = npcScripts.Select(x => x.Id).ToList();
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
                var key = $"commandIds_{player.Id}_{npc.Id}_{scriptId}";
                var commandIds = await _redisDb.StringGet<List<int>>(key);
                if (!commandIds.Contains(scriptCommand.Id))
                {
                    return;
                }
            }

            var checkIf = true;//判断if条件是否为true

            var caseIfStr = scriptCommand.CaseIf;
            if (!string.IsNullOrEmpty(caseIfStr))
            {
                var caseIfs = JsonConvert.DeserializeObject<List<CaseIf>>(caseIfStr);
                foreach (var caseIf in caseIfs)
                {
                    checkIf = await CheckIf(player, caseIf.Condition, caseIf.Attrs);
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
                    var caseThens = JsonConvert.DeserializeObject<List<CaseThen>>(caseThenStr);
                    foreach (var caseThen in caseThens)
                    {
                        await DoCommand(player, npc, scriptId, caseThen.Command, caseThen.Attrs);
                    }
                }
            }
            else
            {
                //执行else分支
                var caseElseStr = scriptCommand.CaseElse;
                if (!string.IsNullOrEmpty(caseElseStr))
                {
                    var caseElses = JsonConvert.DeserializeObject<List<CaseElse>>(caseElseStr);
                    foreach (var caseElse in caseElses)
                    {
                        await DoCommand(player, npc, scriptId, caseElse.Command, caseElse.Attrs);
                    }
                }
            }
        }

        private async Task<bool> CheckIf(PlayerEntity player, string condition, List<CaseAttribute> attrs)
        {
            var field = attrs.FirstOrDefault(x => x.Attr == "Field")?.Val;
            var relation = attrs.FirstOrDefault(x => x.Attr == "Relation")?.Val;
            var value = attrs.FirstOrDefault(x => x.Attr == "Value")?.Val;
            var wareName = attrs.FirstOrDefault(x => x.Attr == "WareName")?.Val;
            var number = attrs.FirstOrDefault(x => x.Attr == "Number")?.Val;


            switch (condition)
            {
                case "角色属性":
                    if (!CheckField(player, field, value, relation))
                    {
                        return false;
                    }
                    break;

                case "拥有物品":
                    if (!await CheckWare(player.Id, wareName, number, relation))
                    {
                        return false;
                    }

                    break;

                case "完成任务":

                    break;

                case "活动记录":

                    break;
            }

            return true;
        }

        private async Task DoCommand(PlayerEntity player, NpcEntity npc, int scriptId, string command,List<CaseAttribute> attrs)
        {
            var title = attrs.FirstOrDefault(x => x.Attr == "Title")?.Val;
            var message = attrs.FirstOrDefault(x => x.Attr == "Message")?.Val;
            var tips = attrs.FirstOrDefault(x => x.Attr == "Tips")?.Val;
            var commandIdStr = attrs.FirstOrDefault(x => x.Attr == "commandId")?.Val;

            //await _bus.RaiseEvent(new DomainNotification($"command= {command}"));
            switch (command)
            {
                case "播放对话":

                    await _mudProvider.ShowMessage(player.Id, message);

                    break;

                case "对话选项":
                    await _mudProvider.ShowMessage(player.Id, $"<a href='javascript:;' class='chat'>{title}</a><br />");
                    break;

                case "输入选项":
                    await _mudProvider.ShowMessage(player.Id, $"<a href = 'javascript:;' class='chat'>{tips}</a><input type = 'text' name='input' style='width:120px;margin-left:10px;' /><input type = 'button' class='input'  value='确定' /><br />");
                    break;


                case "选择选项":
                    await _mudProvider.ShowMessage(player.Id, $"<select><option value =''> 请选择</ option ></select>< input type = 'button' class='input'  value='确定' /><br />");


                    break;

                case "跳转到分支":

                    int.TryParse(commandIdStr,out int commandId);

                    await DoScript(player, npc, scriptId, commandId);

                    break;
            }
        }

        private async Task<bool> CheckWare(int playerId, string wareName, string number, string relation)
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

            return CheckRelation(playerWare.Number, numberValue, relation);

        }

        private bool CheckRelation(int value1,int value2, string relation)
        {
            switch (relation)
            {
                case "等于": return value1 == value2;
                case "不等于": return value1 != value2;
                case "大于": return value1 > value2;
                case "大于等于": return value1 >= value2;
                case "小于": return value1 < value2;
                case "小于等于": return value1 <= value2;
                default: return false;
            }
        }

        private bool CheckField(PlayerEntity player, string field, string value, string relation)
        {
            try
            {
                var fieldValue = GetFieldValue(player, field);

                int.TryParse(value, out int attrValue);

                return CheckRelation(fieldValue, attrValue, relation);
            }
            catch (Exception)
            {
                return false;
            }
        }


        private int GetFieldValue(PlayerEntity player, string field)
        {
            var fieldEnum = (PlayerConditionFieldEnum)Enum.Parse(typeof(PlayerConditionFieldEnum), field, true);
            switch (fieldEnum)
            {
                case PlayerConditionFieldEnum.先天悟性:
                    return player.Int;

                case PlayerConditionFieldEnum.等级:
                    return player.Level;

                default:
                    throw new Exception($"属性 {field} 不存在");
            }
        }


        #endregion

    }
}
