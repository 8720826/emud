using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Services;
using Emprise.Domain.Player.Entity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Emprise.Domain.Player.Services
{
    public class PlayerDomainService : BaseDomainService<PlayerEntity>, IPlayerDomainService
    {
        private readonly IRepository<PlayerEntity> _playerRepository;

        public PlayerDomainService(IRepository<PlayerEntity> playerRepository, IMemoryCache cache, IMediatorHandler bus) : base(playerRepository, cache, bus)
        {
            _playerRepository = playerRepository;
        }

    }
}
