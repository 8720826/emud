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

        public async Task<IQueryable<PlayerEntity>> GetAll()
        {
            return await _playerRepository.GetAll();
        }

        public async Task<PlayerEntity> Get(Expression<Func<PlayerEntity, bool>> where)
        {
            return await _playerRepository.Get(where);
        }

        public async Task<PlayerEntity> Get(int id)
        {
            return await _playerRepository.Get(id);
        }

        public async Task Add(PlayerEntity player)
        {
            await _playerRepository.Add(player);
        }

        public async Task Update(PlayerEntity player)
        {
             await _playerRepository.Update(player);
        }

        public async Task Delete(PlayerEntity player)
        {
            await _playerRepository.Remove(player);
        }

        public async Task<PlayerEntity> GetUserPlayer(int userId)
        {
            return await _playerRepository.Get(x => x.UserId == userId);
        }

        public async Task<PlayerEntity> Computed(PlayerEntity player)
        {
            player.MaxHp = (player.Str * 3 + player.StrAdd) * 10 + player.Level * 5 + 100;
            player.LimitMp = ((player.Con * 4 + player.ConAdd) * 10 + player.Level * 5 + 100) * 10;
            if (player.MaxMp * 10 < player.LimitMp)
            {
                player.MaxMp = player.LimitMp / 10;
            }

            return await Task.FromResult(player);
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
