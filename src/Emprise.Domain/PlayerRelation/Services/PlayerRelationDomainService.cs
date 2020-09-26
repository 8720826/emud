using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Services;
using Emprise.Domain.PlayerRelation.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.PlayerRelation.Services
{

    public class PlayerRelationDomainService : BaseDomainService<PlayerRelationEntity>, IPlayerRelationDomainService
    {
        private readonly IRepository<PlayerRelationEntity> _playerRelationRepository;

        public PlayerRelationDomainService(IRepository<PlayerRelationEntity> playerRelationRepository):base(playerRelationRepository)
        {
            _playerRelationRepository = playerRelationRepository;
        }

    }
}
