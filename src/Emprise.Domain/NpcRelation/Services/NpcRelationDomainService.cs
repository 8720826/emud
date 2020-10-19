using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Services;
using Emprise.Domain.NpcRelation.Entity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.NpcRelation.Services
{
    public class NpcRelationDomainService: BaseDomainService<NpcRelationEntity>, INpcRelationDomainService
    {
        private readonly IRepository<NpcRelationEntity> _npcRelationRepository;

        public NpcRelationDomainService(IRepository<NpcRelationEntity> npcRelationRepository, IMemoryCache cache, IMediatorHandler bus) 
            : base(npcRelationRepository, cache, bus)
        {
            _npcRelationRepository = npcRelationRepository;
        }


    }
}
