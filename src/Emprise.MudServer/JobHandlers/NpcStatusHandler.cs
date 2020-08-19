using Emprise.Application.Player.Services;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Interfaces.Ioc;
using Emprise.Domain.Core.Queue.Models;
using Emprise.Domain.ItemDrop.Models;
using Emprise.Domain.ItemDrop.Services;
using Emprise.Domain.Npc.Entity;
using Emprise.Domain.Npc.Services;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.Player.Services;
using Emprise.Domain.Room.Services;
using Emprise.Domain.Skill.Services;
using Emprise.Domain.Ware.Entity;
using Emprise.Domain.Ware.Services;
using Emprise.MudServer.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.MudServer.Handles
{
    public interface INpcStatusHandler : IScoped
    {
        Task Execute(NpcStatusModel model);
    }
    public class NpcStatusHandler : INpcStatusHandler
    {
        private readonly IMudProvider _mudProvider;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly ILogger<NpcStatusHandler> _logger;
        private readonly IRecurringQueue _recurringQueue;
        private readonly IMediatorHandler _bus;
        private readonly IMudOnlineProvider _mudOnlineProvider;
        private readonly IRoomItemDropDomainService  _roomItemDropDomainService;
        private readonly IItemDropDomainService  _itemDropDomainService;
        private readonly IItemDropRateDomainService  _itemDropRateDomainService;
        private readonly IWareDomainService _wareDomainService;
        private readonly IPlayerWareDomainService _playerWareDomainService;
        private readonly ISkillDomainService _skillDomainService;
        private readonly IPlayerSkillDomainService _playerSkillDomainService;
        private readonly INpcDomainService _npcDomainService;

        public NpcStatusHandler(
            IMudProvider mudProvider, 
            IPlayerDomainService playerDomainService, 
            ILogger<NpcStatusHandler> logger, 
            IRecurringQueue recurringQueue,
            IMudOnlineProvider mudOnlineProvider,
           IRoomItemDropDomainService roomItemDropDomainService,
           IItemDropDomainService itemDropDomainService,
           IItemDropRateDomainService itemDropRateDomainService,
           IWareDomainService wareDomainService,
           IPlayerWareDomainService playerWareDomainService,
           ISkillDomainService skillDomainService,
           IPlayerSkillDomainService playerSkillDomainService,
           INpcDomainService npcDomainService,
            IMediatorHandler bus)
        {
            _mudProvider = mudProvider;
            _playerDomainService = playerDomainService;
            _logger = logger;
            _recurringQueue = recurringQueue;
            _bus = bus;
            _mudOnlineProvider = mudOnlineProvider;
            _roomItemDropDomainService = roomItemDropDomainService;
            _itemDropDomainService = itemDropDomainService;
            _itemDropRateDomainService = itemDropRateDomainService;
            _wareDomainService = wareDomainService;
            _playerWareDomainService = playerWareDomainService;
            _skillDomainService = skillDomainService;
            _playerSkillDomainService = playerSkillDomainService;
            _npcDomainService = npcDomainService;
        }
        public async Task Execute(NpcStatusModel model)
        {
            int playerId = model.PlayerId;
            int targetId = model.TargetId;
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                return;
            }
            var npc = await _npcDomainService.Get(targetId);
            if (npc == null)
            {
                return;
            }

            await _mudProvider.ShowMessage(player.Id, $"【切磋】[{npc.Name}]正在攻击你。。。");

        }





    }
}
