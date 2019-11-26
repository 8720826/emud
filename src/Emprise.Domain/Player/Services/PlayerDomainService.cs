using Emprise.Domain.Core.Data;
using Emprise.Domain.Player.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Emprise.Domain.Player.Services
{
    public class PlayerDomainService : IPlayerDomainService
    {
        private readonly IRepository<PlayerEntity> _playerRepository;

        public PlayerDomainService(IRepository<PlayerEntity> playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public async Task<List<PlayerEntity>> Query(Expression<Func<PlayerEntity, bool>> where)
        {
            var query = await _playerRepository.GetAll(where);
            return query.ToList();
        }

        public async Task<PlayerEntity> Get(Expression<Func<PlayerEntity, bool>> where)
        {
            return await _playerRepository.Get(where);
        }

        public async Task<PlayerEntity> Get(int id)
        {
            return await _playerRepository.Get(id);
        }

        public async Task Add(PlayerEntity user)
        {
            await _playerRepository.Add(user);
        }

        public async Task Update(PlayerEntity user)
        {
             await _playerRepository.Update(user);
        }

        public async Task<PlayerEntity> GetUserPlayer(int userId)
        {
            return await _playerRepository.Get(x => x.UserId == userId);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
