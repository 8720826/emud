using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
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
    public class NpcScriptDomainService : BaseDomainService<NpcScriptEntity>, INpcScriptDomainService
    {
        private readonly IRepository<NpcScriptEntity> _npcScriptRepository;

        public NpcScriptDomainService(IRepository<NpcScriptEntity> npcScriptRepository, IMemoryCache cache, IMediatorHandler bus) : base(npcScriptRepository, cache, bus)
        {
            _npcScriptRepository = npcScriptRepository;
        }



    }
}
