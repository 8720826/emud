using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Email.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Email.Services
{
    public class PlayerEmailDomainService : IPlayerEmailDomainService
    {
        private readonly IRepository<PlayerEmailEntity> _playerEmailRepository;

        public PlayerEmailDomainService(IRepository<PlayerEmailEntity> playerEmailRepository)
        {
            _playerEmailRepository = playerEmailRepository;
        }

        public async Task<IQueryable<PlayerEmailEntity>> Query()
        {
            return await _playerEmailRepository.GetAll();
        }

        public async Task<List<PlayerEmailEntity>> GetMyReceivedEmails(int playerId)
        {
            var query = await _playerEmailRepository.GetAll(x => x.PlayerId == playerId);
            return query.ToList();
        }


        public async Task Add(PlayerEmailEntity item)
        {
            await _playerEmailRepository.Add(item);
        }


        public async Task<int> GetUnreadCount(int playerId)
        {
            var query = await _playerEmailRepository.GetAll(x=>x.PlayerId== playerId && x.Status== EmailStatusEnum.未读);
            return query.Count();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
