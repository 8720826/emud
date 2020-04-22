using AutoMapper;
using Emprise.Application.Npc.Models;
using Emprise.Domain.Core.Authorization;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Enum;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Notifications;
using Emprise.Domain.Npc.Entity;
using Emprise.Domain.Npc.Events;
using Emprise.Domain.Npc.Services;
using Emprise.Domain.Player.Services;
using Emprise.Domain.Quest.Models;
using Emprise.Domain.Quest.Services;
using Emprise.Infra.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Emprise.Application.User.Services
{
    public class NpcAppService : INpcAppService
    {
        private readonly IMediatorHandler _bus;
        private readonly IMapper _mapper;
        private readonly INpcDomainService _npcDomainService;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly IAccountContext _account;
        private readonly IScriptCommandDomainService _ScriptCommandDomainService;
        private readonly INpcScriptDomainService _npcScriptDomainService;
        private readonly IMudProvider _mudProvider;
        private readonly ILogger<NpcAppService> _logger;

        public NpcAppService(IMediatorHandler bus, IMapper mapper, INpcDomainService npcDomainService, IPlayerDomainService playerDomainService, IAccountContext account, IScriptCommandDomainService ScriptCommandDomainService, INpcScriptDomainService npcScriptDomainService,  IMudProvider mudProvider, ILogger<NpcAppService> logger)
        {
            _bus = bus;
            _mapper = mapper;
            _npcDomainService = npcDomainService;
            _playerDomainService = playerDomainService;
            _account = account;
            _ScriptCommandDomainService = ScriptCommandDomainService;
            _npcScriptDomainService = npcScriptDomainService;
            _mudProvider = mudProvider;
            _logger = logger;
        }

        public async Task<NpcEntity> Get(int id)
        {
            return await _npcDomainService.Get(id);
        }

        public async Task<NpcInfo> GetNpc(int playerId,int id)
        {
            var npcInfo = new NpcInfo()
            {
                Descriptions = new List<string>(),
                Actions = new List<NpcAction>()
            };
            var npc = await _npcDomainService.Get(id);
            if (npc == null)
            {
                return npcInfo;
            }


            npcInfo.Id = id;
            npcInfo.Name = npc.Name;
            string genderStr = npc.Gender.ToGender();

            if(npc.Type == NpcTypeEnum.人物)
            {
                npcInfo.Actions.Add(new NpcAction { Name = NpcActionEnum.给予.ToString() });
            }        

            if (npc.CanFight)
            {
                npcInfo.Actions.Add(new NpcAction { Name = NpcActionEnum.切磋.ToString() });
            }

            if (npc.CanKill)
            {
                npcInfo.Actions.Add(new NpcAction { Name = NpcActionEnum.杀死.ToString() });
            }

            var player = await _playerDomainService.Get(_account.PlayerId);

            npcInfo.Descriptions.Add(npc.Description??"");
            npcInfo.Descriptions.Add($"{genderStr}{npc.Age.ToAge()}");
            npcInfo.Descriptions.Add($"{genderStr}{npc.Per.ToPer(npc.Age, npc.Gender)}");
            npcInfo.Descriptions.Add($"{genderStr}{npc.Exp.ToKunFuLevel(player.Exp)}");


            var npcScripts = await _npcScriptDomainService.Query(x => x.NpcId == npc.Id);
            foreach (var npcScript in npcScripts)
            {
                var scriptCommands = await _ScriptCommandDomainService.Query(x => x.ScriptId == npcScript.Id);

                var actions = scriptCommands.Where(x => x.IsEntry).Select(x => new NpcAction { Name = x.Name, ScriptId = x.ScriptId, CommandId = x.Id }).ToList();

                npcInfo.Actions.AddRange(actions);
            }


            if (!string.IsNullOrEmpty(npc.InitWords))
            {
                var initWords = npc.InitWords.Split('\r').Where(x => !string.IsNullOrEmpty(x)).ToList();
                Random r = new Random();
                int n = r.Next(0, initWords.Count);
                var initWord = initWords[n];
                if (!string.IsNullOrEmpty(initWord))
                {
                    await _mudProvider.ShowMessage(playerId, initWord);
                }
            }

            //await CheckQuest(playerId, npc);


            await _bus.RaiseEvent(new ChatWithNpcEvent(playerId, npc.Id)).ConfigureAwait(false);

            return npcInfo;
        }
        
         


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
