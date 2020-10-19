using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Services;
using Emprise.Domain.Email.Entity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Email.Services
{
    public class PlayerEmailDomainService : BaseDomainService<PlayerEmailEntity>, IPlayerEmailDomainService
    {
        private readonly IRepository<PlayerEmailEntity> _playerEmailRepository;

        public PlayerEmailDomainService(IRepository<PlayerEmailEntity> playerEmailRepository, IMemoryCache cache, IMediatorHandler bus) : base(playerEmailRepository, cache, bus)
        {
            _playerEmailRepository = playerEmailRepository;
        }


        public async Task<List<PlayerEmailEntity>> GetMyReceivedEmails(int playerId)
        {
            var query = await _playerEmailRepository.GetAll(x => x.PlayerId == playerId);
            return query.ToList();
        }



        public async Task<int> GetUnreadCount(int playerId)
        {
            var query = await _playerEmailRepository.GetAll(x=>x.PlayerId== playerId && x.Status== EmailStatusEnum.未读);
            return query.Count();
        }

    }
}
