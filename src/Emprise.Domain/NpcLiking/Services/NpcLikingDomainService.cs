using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Services;
using Emprise.Domain.NpcLiking.Entity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.NpcLiking.Services
{
    public class NpcLikingDomainService : BaseDomainService<NpcLikingEntity>, INpcLikingDomainService
    {
        private readonly IRepository<NpcLikingEntity> _npcRelationRepository;

        public NpcLikingDomainService(IRepository<NpcLikingEntity> npcRelationRepository, IMemoryCache cache, IMediatorHandler bus) 
            : base(npcRelationRepository, cache, bus)
        {
            _npcRelationRepository = npcRelationRepository;
        }


    }
}
