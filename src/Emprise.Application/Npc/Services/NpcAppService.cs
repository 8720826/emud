using AutoMapper;
using Emprise.Domain.Core.Authorization;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Npc.Entity;
using Emprise.Domain.Npc.Services;
using Emprise.Domain.Player.Services;
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
        private readonly IScriptDomainService _scriptDomainService;
        private readonly INpcScriptDomainService _npcScriptDomainService;
        private readonly IMudProvider _mudProvider;
        private readonly ILogger<NpcAppService> _logger;

        public NpcAppService(IMediatorHandler bus, IMapper mapper, INpcDomainService npcDomainService, IPlayerDomainService playerDomainService, IAccountContext account, IScriptDomainService scriptDomainService, INpcScriptDomainService npcScriptDomainService,  IMudProvider mudProvider, ILogger<NpcAppService> logger)
        {
            _bus = bus;
            _mapper = mapper;
            _npcDomainService = npcDomainService;
            _playerDomainService = playerDomainService;
            _account = account;
            _scriptDomainService = scriptDomainService;
            _npcScriptDomainService = npcScriptDomainService;
            _mudProvider = mudProvider;
            _logger = logger;
        }

        public async Task<NpcEntity> Get(int id)
        {
            return await _npcDomainService.Get(id);
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
