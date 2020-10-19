using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Services;
using Emprise.Domain.Npc.Entity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Emprise.Domain.Npc.Services
{
    public class ScriptDomainService : BaseDomainService<ScriptEntity>, IScriptDomainService
    { 
        private readonly IRepository<ScriptEntity> _scriptRepository;
        private readonly IMemoryCache _cache;
        private readonly IMediatorHandler _bus;

        public ScriptDomainService(IRepository<ScriptEntity> scriptRepository, IMemoryCache cache, IMediatorHandler bus) : base(scriptRepository, cache, bus)
        {
            _scriptRepository = scriptRepository;
            _cache = cache;
            _bus = bus;
        }

    }
}
