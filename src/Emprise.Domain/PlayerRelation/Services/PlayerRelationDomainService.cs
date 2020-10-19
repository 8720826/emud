using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Services;
using Emprise.Domain.PlayerRelation.Entity;
using Microsoft.Extensions.Caching.Memory;
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

        public PlayerRelationDomainService(IRepository<PlayerRelationEntity> playerRelationRepository, IMemoryCache cache, IMediatorHandler bus) : base(playerRelationRepository, cache, bus)
        {
            _playerRelationRepository = playerRelationRepository;
        }

    }
}
