using AutoMapper;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.CommandHandlers;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Notifications;
using Emprise.Domain.Player.Services;
using Emprise.Domain.Quest.Services;
using Emprise.Domain.Skill.Models;
using Emprise.Domain.Skill.Services;
using Emprise.Domain.Ware.Services;
using Emprise.MudServer.Commands;
using Emprise.MudServer.Commands.SkillCommands;
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
        IRequestHandler<ShowSkillDetailCommand, Unit>

        
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


        
    }
}
