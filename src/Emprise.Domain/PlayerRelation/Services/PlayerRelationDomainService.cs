using Emprise.Domain.Core.Data;
using Emprise.Domain.PlayerRelation.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.PlayerRelation.Services
{

    public class PlayerRelationDomainService : IPlayerRelationDomainService
    {
        private readonly IRepository<PlayerRelationEntity> _playerRelationRepository;

        public PlayerRelationDomainService(IRepository<PlayerRelationEntity> playerRelationRepository)
        {
            _playerRelationRepository = playerRelationRepository;
        }

        public async Task<IQueryable<PlayerRelationEntity>> GetAll()
        {
            return await _playerRelationRepository.GetAll();
        }

        public async Task<PlayerRelationEntity> Get(Expression<Func<PlayerRelationEntity, bool>> where)
        {
            return await _playerRelationRepository.Get(where);
        }

        public async Task<PlayerRelationEntity> Get(int id)
        {
            return await _playerRelationRepository.Get(id);
        }

        public async Task Add(PlayerRelationEntity player)
        {
            await _playerRelationRepository.Add(player);
        }

        public async Task Update(PlayerRelationEntity player)
        {
            await _playerRelationRepository.Update(player);
        }

        public async Task Delete(PlayerRelationEntity player)
        {
            await _playerRelationRepository.Remove(player);
        }
    }
}
