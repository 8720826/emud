using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Services;
using Emprise.Domain.Player.Entity;
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

        public PlayerDomainService(IRepository<PlayerEntity> playerRepository) : base(playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
