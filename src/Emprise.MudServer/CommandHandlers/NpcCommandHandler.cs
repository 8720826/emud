using AutoMapper;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.CommandHandlers;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
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
using Emprise.Domain.Npc.Models;
using Emprise.Domain.Npc.Entity;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.Player.Services;
using Emprise.Domain.Quest.Services;
using System.Reflection;
using Emprise.Domain.Core.Attributes;
using Emprise.Domain.Ware.Services;
using Emprise.Domain.Quest.Entity;
using Emprise.Domain.Quest.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Emprise.MudServer.CommandHandlers
{
    
    public class NpcCommandHandler : CommandHandler, 
        IRequestHandler<ShowNpcCommand, Unit>

    {
        private readonly IMediatorHandler _bus;
        private readonly ILogger<NpcCommandHandler> _logger;
        private readonly INpcDomainService _npcDomainService;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly IScriptDomainService _scriptDomainService;
        private readonly INpcScriptDomainService _npcScriptDomainService;
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


            var npcScripts = await _npcScriptDomainService.Query(x => x.NpcId == npc.Id);
            foreach (var npcScript in npcScripts)
            {
                var script = await _scriptDomainService.Get(npcScript.ScriptId);

                if (script != null)
                {
                    npcInfo.Actions.Add(new NpcAction { Name = script.Name, ScriptId = script.Id, CommandId = 0 });
                }

            }


            await _mudProvider.ShowNpc(playerId, npcInfo);


            await _bus.RaiseEvent(new ChatWithNpcEvent(playerId, npc.Id)).ConfigureAwait(false);



            return Unit.Value;
        }


    }
}
