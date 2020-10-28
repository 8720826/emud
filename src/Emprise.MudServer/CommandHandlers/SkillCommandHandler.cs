using AutoMapper;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.CommandHandlers;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Notifications;
using Emprise.Domain.Npc.Services;
using Emprise.Domain.Player.Models;
using Emprise.Domain.Player.Services;
using Emprise.Domain.PlayerRelation.Services;
using Emprise.Domain.Quest.Services;
using Emprise.Domain.Skill.Entity;
using Emprise.Domain.Skill.Models;
using Emprise.Domain.Skill.Services;
using Emprise.Domain.Ware.Services;
using Emprise.MudServer.Commands;
using Emprise.MudServer.Commands.NpcCommands;
using Emprise.MudServer.Commands.SkillCommands;
using Emprise.MudServer.Commands.WareCommands;
using Emprise.MudServer.Events;
using Emprise.MudServer.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emprise.MudServer.CommandHandlers
{
    public class SkillCommandHandler : CommandHandler,
        IRequestHandler<ShowMySkillCommand, Unit>,
        IRequestHandler<ShowSkillDetailCommand, Unit>,
        IRequestHandler<ShowFriendSkillCommand, Unit>,
        IRequestHandler<ShowNpcSkillCommand, Unit>,
        IRequestHandler<LearnSkillCommand, Unit>,
        IRequestHandler<LearnWareCommand, Unit>

    {
        private readonly IMediatorHandler _bus;
        private readonly ILogger<SkillCommandHandler> _logger;
        private readonly ISkillDomainService _skillDomainService;
        private readonly IWareDomainService _wareDomainService;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly IMapper _mapper;
        private readonly IMail _mail;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly IPlayerSkillDomainService _playerSkillDomainService;
        private readonly INpcDomainService _npcDomainService;
        private readonly IPlayerRelationDomainService _playerRelationDomainService;
        private readonly INpcSkillDomainService _npcSkillDomainService;
        private readonly IPlayerWareDomainService _playerWareDomainService;
        private readonly IRedisDb _redisDb;
        private readonly IMudProvider _mudProvider;
       public SkillCommandHandler(
            IMediatorHandler bus,
            ILogger<SkillCommandHandler> logger,
            ISkillDomainService skillDomainService,
            IWareDomainService wareDomainService,
            IHttpContextAccessor httpAccessor,
            IMapper mapper,
            IMail mail,
            IPlayerDomainService playerDomainService,
            IPlayerSkillDomainService playerSkillDomainService,
            INpcDomainService npcDomainService,
            IPlayerRelationDomainService playerRelationDomainService,
            INpcSkillDomainService npcSkillDomainService,
            IPlayerWareDomainService playerWareDomainService,
            IRedisDb redisDb,
            IMudProvider mudProvider,
            INotificationHandler<DomainNotification> notifications,
            IUnitOfWork uow) : base(uow, bus, notifications)
        {

            _bus = bus;
            _logger = logger;
            _skillDomainService = skillDomainService;
            _wareDomainService = wareDomainService;
            _httpAccessor = httpAccessor;
            _mapper = mapper;
            _mail = mail;
            _playerDomainService = playerDomainService;
            _playerSkillDomainService = playerSkillDomainService;
            _npcDomainService = npcDomainService;
            _playerRelationDomainService = playerRelationDomainService;
            _npcSkillDomainService = npcSkillDomainService;
            _playerWareDomainService = playerWareDomainService;
            _redisDb = redisDb;
            _mudProvider = mudProvider;
        }


        public async Task<Unit> Handle(ShowMySkillCommand command, CancellationToken cancellationToken)
        {
            var playerId = command.PlayerId;
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                return Unit.Value;
            }

            var skillModels = new List<SkillModel>();

            var playerSkills = await _playerSkillDomainService.GetAll(player.Id);

            var baseSkills = await _skillDomainService.GetAllBaseSkills();
            foreach (var baseSkill in baseSkills)
            {
                if(playerSkills.Count(x=>x.SkillId== baseSkill.Id) == 0)
                {
                    var playerSkill = new PlayerSkillEntity
                    {
                        Exp = 0,
                        Level = 0,
                        PlayerId = playerId,
                        SkillId = baseSkill.Id,
                        SkillName = baseSkill.Name
                    };
                    await _playerSkillDomainService.Add(playerSkill);
                    playerSkills.Add(playerSkill);
                }
            }


            var ids = playerSkills?.Select(x => x.SkillId);

            var skills = (await _skillDomainService.GetAll()).Where(x => ids.Contains(x.Id));
            foreach (var playerSkill in playerSkills)
            {
                var skill = skills.FirstOrDefault(x => x.Id == playerSkill.SkillId);
                if (skill != null)
                {
                    var skillModel = _mapper.Map<SkillModel>(skill);
                    skillModel.ObjectSkillId = playerSkill.Id;
                    skillModel.Level = playerSkill.Level;
                    skillModel.Exp = playerSkill.Exp;
                    skillModels.Add(skillModel);
                }

            }

            await _mudProvider.ShowMySkill(playerId, skillModels);
            return Unit.Value;
        }

        public async Task<Unit> Handle(ShowSkillDetailCommand command, CancellationToken cancellationToken)
        {
            var playerId = command.PlayerId;
            var skillId = command.MySkillId;
            var type = command.Type;


            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                return Unit.Value;
            }

            if (type == 1)
            {

                var playerSkill = await _playerSkillDomainService.Get(skillId);
                if (playerSkill == null)
                {
                    return Unit.Value;
                }

                var skill = await _skillDomainService.Get(playerSkill.SkillId);
                if (skill == null)
                {
                    return Unit.Value;
                }

                var skillModel = _mapper.Map<SkillModel>(skill);
                skillModel.ObjectSkillId = playerSkill.Id;
                skillModel.Level = playerSkill.Level;
                skillModel.Exp = playerSkill.Exp;
                skillModel.ObjectType = 1;
                await _mudProvider.ShowSkill(playerId, skillModel);
            }
            else
            {

                var npcSkill = await _npcSkillDomainService.Get(skillId);
                if (npcSkill == null)
                {
                    return Unit.Value;
                }

                var skill = await _skillDomainService.Get(npcSkill.SkillId);
                if (skill == null)
                {
                    return Unit.Value;
                }

                var skillModel = _mapper.Map<SkillModel>(skill);
                skillModel.ObjectSkillId = npcSkill.Id;
                skillModel.Level = npcSkill.Level;
                skillModel.Exp = npcSkill.Exp;
                skillModel.ObjectType = 2;
                await _mudProvider.ShowSkill(playerId, skillModel);
            }

            return Unit.Value;
        }

        public async Task<Unit> Handle(ShowFriendSkillCommand command, CancellationToken cancellationToken)
        {
            var playerId = command.PlayerId;
            var friendId = command.FriendId;
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                return Unit.Value;
            }

            var friend = await _playerDomainService.Get(friendId);
            if (friend == null)
            {
                return Unit.Value;
            }

            var friendSkillInfoModel = new FriendSkillInfoModel
            {
                Player = _mapper.Map<PlayerBaseInfo>(friend)
            };

            var skillModels = new List<FriendSkillModel>();
            var friendSkills = await _playerSkillDomainService.GetAll(friendId);
            var friendSkillIds = friendSkills?.Select(x => x.SkillId);

            var mySkills = await _playerSkillDomainService.GetAll(playerId);

            var skills = (await _skillDomainService.GetAll()).Where(x => friendSkillIds.Contains(x.Id));
            foreach (var friendSkill in friendSkills)
            {
                var mySkill = mySkills.FirstOrDefault(x => x.SkillId == friendSkill.SkillId);

                var skill = skills.FirstOrDefault(x => x.Id == friendSkill.SkillId);
                if (skill != null)
                {
                    var skillModel = _mapper.Map<FriendSkillModel>(skill);
                    skillModel.ObjectSkillId = friendSkill.Id;
                    skillModel.Level = friendSkill.Level;
                    skillModel.Exp = friendSkill.Exp;

                    skillModel.MyExp = mySkill?.Exp ?? 0;
                    skillModel.MyLevel = mySkill?.Exp ?? 0;

                    skillModels.Add(skillModel);
                }

            }

            friendSkillInfoModel.Skills = skillModels;

            await _mudProvider.ShowFriendSkill(playerId, friendSkillInfoModel);


            return Unit.Value;
        }


        public async Task<Unit> Handle(ShowNpcSkillCommand command, CancellationToken cancellationToken)
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



            var playerRelation = await _playerRelationDomainService.Get(x => x.Type == PlayerRelationTypeEnum.师父 && x.PlayerId == playerId && x.RelationId == npcId);

            if (playerRelation == null)
            {
                return Unit.Value;
            }

            var npcSkillInfoModel = new NpcSkillInfoModel
            {
                Npc = _mapper.Map<NpcBaseInfo>(npc)
            };

            var skillModels = new List<NpcSkillModel>();

            var npcSkills = await _npcSkillDomainService.GetAll(npcId);
            var npcSkillIds = npcSkills?.Select(x => x.SkillId);

            var mySkills = await _playerSkillDomainService.GetAll(playerId);

            var skills = (await _skillDomainService.GetAll()).Where(x => npcSkillIds.Contains(x.Id));

            foreach (var npcSkill in npcSkills)
            {
                var mySkill = mySkills.FirstOrDefault(x => x.SkillId == npcSkill.SkillId);

                var skill = skills.FirstOrDefault(x => x.Id == npcSkill.SkillId);
                if (skill != null)
                {
                    var skillModel = _mapper.Map<NpcSkillModel>(skill);
                    skillModel.ObjectSkillId = npcSkill.Id;
                    skillModel.Level = npcSkill.Level;
                    skillModel.Exp = npcSkill.Exp;

                    skillModel.MyExp = mySkill?.Exp ?? 0;
                    skillModel.MyLevel = mySkill?.Exp ?? 0;

                    skillModels.Add(skillModel);
                }

            }

            npcSkillInfoModel.Skills = skillModels;

            await _mudProvider.ShowNpcSkill(playerId, npcSkillInfoModel);


            return Unit.Value;
        }


        public async Task<Unit> Handle(LearnSkillCommand command, CancellationToken cancellationToken)
        {
            var playerId = command.PlayerId;
            var objectSkillId = command.MySkillId;
            var type = command.Type;
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                return Unit.Value;
            }

            int skillId = 0;
            int objectSkillLevel = 0;

            if (type == 1)
            {
                var objectSkill = await _playerSkillDomainService.Get(objectSkillId);
                if (objectSkill == null)
                {
                    return Unit.Value;
                }
                skillId = objectSkill.SkillId;
                objectSkillLevel = objectSkill.Level;
            }
            else
            {
                var objectSkill = await _npcSkillDomainService.Get(objectSkillId);
                if (objectSkill == null)
                {
                    return Unit.Value;
                }
                skillId = objectSkill.SkillId;
                objectSkillLevel = objectSkill.Level;
            }


            var skill = await _skillDomainService.Get(skillId);
            if (skill == null)
            {
                return Unit.Value;
            }

            if (player.Pot <= 0)
            {
                await _mudProvider.ShowMessage(player.Id, $"潜能不够，无法修练！");
                return Unit.Value;
            }

            var needPot = 0;
            var exp = 0;

            var mySkill = await _playerSkillDomainService.Get(x => x.PlayerId == playerId && x.SkillId == skillId);
            if (mySkill == null)
            {
                needPot = 100;
                exp = 1;
                if (player.Pot <= needPot)
                {
                    await _mudProvider.ShowMessage(player.Id, $"潜能不够，无法修练！");
                    return Unit.Value;
                }

                mySkill = new PlayerSkillEntity
                {
                    Exp = 1,
                    Level = 1,
                    SkillId = skillId,
                    PlayerId = playerId,
                    SkillName = skill.Name
                };
                await _playerSkillDomainService.Add(mySkill);

                player.Pot -= needPot;
                await _playerDomainService.Update(player);

                await _mudProvider.ShowMessage(player.Id, $"你消耗潜能{needPot}，学会了[{skill.Name}]！");
                return Unit.Value;
            }


            if (player.Level * 10 < mySkill.Level)
            {
                await _mudProvider.ShowMessage(player.Id, $"武功最高等级不能超过自身等级的10倍！");
                return Unit.Value;
            }

            if (mySkill.Level >= objectSkillLevel && !skill.IsBase)
            {
                await _mudProvider.ShowMessage(player.Id, $"你已经无法从对方身上学到什么了！");
                return Unit.Value;
            }

            var @int = player.Int * 3 + player.IntAdd;
            var nextLevelExp = (mySkill.Level + 1) * (mySkill.Level + 1) * 50;
            var levelUpExp = nextLevelExp - mySkill.Level * mySkill.Level * 50;

            Random random = new Random();
            exp = random.Next(1, levelUpExp / 10);
            needPot = exp * 100 / @int;

            var effect = exp * 1000 / levelUpExp;

            string effectWords;
            if (effect > 80)
            {
                effectWords = "恍然大悟";
            }
            else if (effect > 50)
            {
                effectWords = "有所顿悟";
            }
            else if (effect > 20)
            {
                effectWords = "略有所获";
            }
            else if (effect > 10)
            {
                effectWords = "略有所获";
            }
            else
            {
                effectWords = "似乎没有什么进展";
            }

            player.Pot -= needPot;
            await _playerDomainService.Update(player);
            mySkill.SkillName = skill.Name;
            mySkill.Exp += exp;
            await _mudProvider.ShowMessage(player.Id, $"你消耗潜能{needPot}，{effectWords}，[{skill.Name}]经验增加{exp}！");

            if (mySkill.Exp > nextLevelExp)
            {
                mySkill.Level++;
                await _mudProvider.ShowMessage(player.Id, $"[{skill.Name}]等级上升为 Lv.{mySkill.Level}！");
            }



            await _playerSkillDomainService.Update(mySkill);



            await _bus.RaiseEvent(new PlayerAttributeChangedEvent(player)).ConfigureAwait(false);
            return Unit.Value;
        }


        public async Task<Unit> Handle(LearnWareCommand command, CancellationToken cancellationToken)
        {
            var myWareId = command.MyWareId;
            var playerId = command.PlayerId;

            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            var playerWare = await _playerWareDomainService.Get(myWareId);
            if (playerWare == null || playerWare.PlayerId != playerId)
            {
                await _bus.RaiseEvent(new DomainNotification($"你什么也没学会！"));
                return Unit.Value;
            }

            var ware = await _wareDomainService.Get(playerWare.WareId);
            if (ware == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"你什么也没学会！"));
                await _playerWareDomainService.Delete(playerWare);
                return Unit.Value;
            }

            if (ware.Category != WareCategoryEnum.秘籍)
            {
                await _bus.RaiseEvent(new DomainNotification($"你什么也没学会！"));
                return Unit.Value;
            }

            var skill = await _skillDomainService.Get(x => x.Name == ware.Name);
            if (skill == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"你什么也没学会！"));
                return Unit.Value;
            }

            if (player.Pot <= 0)
            {
                await _mudProvider.ShowMessage(player.Id, $"潜能不够，无法修练！");
                return Unit.Value;
            }

            var needPot = 0;
            var exp = 0;

            var mySkill = await _playerSkillDomainService.Get(x => x.PlayerId == playerId && x.SkillId == skill.Id);
            if (mySkill == null)
            {
                needPot = 100;
                exp = 1;
                if (player.Pot <= needPot)
                {
                    await _mudProvider.ShowMessage(player.Id, $"潜能不够，无法修练！");
                    return Unit.Value;
                }

                mySkill = new PlayerSkillEntity
                {
                    Exp = 1,
                    Level = 1,
                    SkillId = skill.Id,
                    PlayerId = playerId,
                    SkillName = skill.Name
                };
                await _playerSkillDomainService.Add(mySkill);

                player.Pot -= needPot;
                await _playerDomainService.Update(player);

                await _mudProvider.ShowMessage(player.Id, $"你消耗潜能{needPot}，学会了[{skill.Name}]！");
                return Unit.Value;
            }


            if (player.Level * 10 < mySkill.Level)
            {
                await _mudProvider.ShowMessage(player.Id, $"武功最高等级不能超过自身等级的10倍！");
                return Unit.Value;
            }

            if (mySkill.Level >= playerWare.Level && !skill.IsBase)
            {
                await _mudProvider.ShowMessage(player.Id, $"你已经无法从秘籍上学到什么了！");
                return Unit.Value;
            }

            var @int = player.Int * 3 + player.IntAdd;
            var nextLevelExp = (mySkill.Level + 1) * (mySkill.Level + 1) * 50;
            var levelUpExp = nextLevelExp - mySkill.Level * mySkill.Level * 50;

            Random random = new Random();
            exp = random.Next(1, levelUpExp / 10);
            needPot = exp * 100 / @int;

            var effect = exp * 1000 / levelUpExp;

            string effectWords;
            if (effect > 80)
            {
                effectWords = "恍然大悟";
            }
            else if (effect > 50)
            {
                effectWords = "有所顿悟";
            }
            else if (effect > 20)
            {
                effectWords = "略有所获";
            }
            else if (effect > 10)
            {
                effectWords = "略有所获";
            }
            else
            {
                effectWords = "似乎没有什么进展";
            }

            player.Pot -= needPot;
            await _playerDomainService.Update(player);
            mySkill.SkillName = skill.Name;
            mySkill.Exp += exp;
            await _mudProvider.ShowMessage(player.Id, $"你消耗潜能{needPot}，{effectWords}，[{skill.Name}]经验增加{exp}！");

            if (mySkill.Exp > nextLevelExp)
            {
                mySkill.Level++;
                await _mudProvider.ShowMessage(player.Id, $"[{skill.Name}]等级上升为 Lv.{mySkill.Level}！");
            }



            await _playerSkillDomainService.Update(mySkill);



            await _bus.RaiseEvent(new PlayerAttributeChangedEvent(player)).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
