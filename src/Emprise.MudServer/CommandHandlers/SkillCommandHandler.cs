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
        IRequestHandler<ShowNpcSkillCommand, Unit>

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
                    skillModel.PlayerSkillId = playerSkill.Id;
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
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                return Unit.Value;
            }

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
            skillModel.PlayerSkillId = playerSkill.Id;
            skillModel.Level = playerSkill.Level;
            skillModel.Exp = playerSkill.Exp;

            await _mudProvider.ShowSkill(playerId, skillModel);

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
                    skillModel.PlayerSkillId = friendSkill.Id;
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

            var skillModels = new List<FriendSkillModel>();

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
                    var skillModel = _mapper.Map<FriendSkillModel>(skill);
                    skillModel.PlayerSkillId = npcSkill.Id;
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
    }
}
