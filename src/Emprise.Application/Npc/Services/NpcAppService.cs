using AutoMapper;
using Emprise.Application.Npc.Models;
using Emprise.Domain.Core.Authorization;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Enum;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Npc.Entity;
using Emprise.Domain.Npc.Services;
using Emprise.Domain.Player.Services;
using Emprise.Infra.Extensions;
using Microsoft.Extensions.DependencyInjection;
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
        public NpcAppService(IMediatorHandler bus, IMapper mapper, INpcDomainService npcDomainService, IPlayerDomainService playerDomainService, IAccountContext account, IScriptCommandDomainService ScriptCommandDomainService, INpcScriptDomainService npcScriptDomainService, IMudProvider mudProvider)
        {
            _bus = bus;
            _mapper = mapper;
            _npcDomainService = npcDomainService;
            _playerDomainService = playerDomainService;
            _account = account;
            _ScriptCommandDomainService = ScriptCommandDomainService;
            _npcScriptDomainService = npcScriptDomainService;
            _mudProvider = mudProvider;
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
                Actions = new List<string>()
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
                npcInfo.Actions.Add(NpcActionEnum.给予.ToString());
            }        

            if (npc.CanFight)
            {
                npcInfo.Actions.Add(NpcActionEnum.切磋.ToString());
            }

            if (npc.CanKill)
            {
                npcInfo.Actions.Add(NpcActionEnum.杀死.ToString());
            }

            var player = await _playerDomainService.Get(_account.PlayerId);

            npcInfo.Descriptions.Add(npc.Description??"");
            npcInfo.Descriptions.Add($"{genderStr}{npc.Age.ToAge()}");
            npcInfo.Descriptions.Add($"{genderStr}{npc.Per.ToPer(npc.Age, npc.Gender)}");
            npcInfo.Descriptions.Add($"{genderStr}{npc.Exp.ToKunFuLevel(player.Exp)}");

            var scriptIds = npc.NpcScripts.Select(x => x.Id).ToList();

            var npcScripts = await _npcScriptDomainService.Query(x => scriptIds.Contains(x.Id));
            foreach (var npcScript in npcScripts)
            {
                var scriptCommands = await _ScriptCommandDomainService.Query(x => x.ScriptId == npcScript.Id);

                var actions = scriptCommands.Where(x => x.IsEntry).Select(x => x.ActionName).ToList();

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

            /*
            var type = Type.GetType("Emprise.MudServer.Scripts." + npc.Script + ",Emprise.MudServer", false, true);
            if (type != null)
            {
                using (var serviceScope = _services.CreateScope())
                {
                     var argtypes = type.GetConstructors()
                     .First()
                     .GetParameters()
                     .Select(x =>
                     {
                         if (x.Name == "player")
                             return player;
                         else if (x.Name == "npc")
                             return npc;
                         else if (x.ParameterType == typeof(IServiceProvider))
                             return serviceScope.ServiceProvider;
                         else
                             return serviceScope.ServiceProvider.GetService(x.ParameterType);
                     })
                     .ToArray();

                    var job = Activator.CreateInstance(type, argtypes);

                    MethodInfo method = type.GetMethod("GetActions");
                    var actions =  method.Invoke(job, new object[] { }) as Task<List<string>>; ;


                    npcInfo.Actions.AddRange(actions.Result);
                }

            }

            */

            return npcInfo;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
