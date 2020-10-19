using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
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
    public class NpcDomainService : BaseDomainService<NpcEntity>, INpcDomainService
    {
        private readonly IRepository<NpcEntity> _npcRepository;

        public NpcDomainService(
            IRepository<NpcEntity> npcRepository, 
            IMemoryCache cache, IMediatorHandler bus) : base(npcRepository, cache, bus)
        {
            _npcRepository = npcRepository;
        }


    }
}
