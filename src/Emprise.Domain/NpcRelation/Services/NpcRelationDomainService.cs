using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Services;
using Emprise.Domain.NpcRelation.Entity;
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

        public NpcRelationDomainService(IRepository<NpcRelationEntity> npcRelationRepository)
            :base(npcRelationRepository)
        {
            _npcRelationRepository = npcRelationRepository;
        }


    }
}
