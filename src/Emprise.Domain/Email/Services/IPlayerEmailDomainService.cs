using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Email.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Email.Services
{
    public interface IPlayerEmailDomainService : IBaseService
    {
        Task<IQueryable<PlayerEmailEntity>> Query();

        Task<List<PlayerEmailEntity>> GetMyReceivedEmails(int playerId);

        Task Add(PlayerEmailEntity item);

        Task<int> GetUnreadCount(int playerId);

        Task<PlayerEmailEntity> Get(int id);

        Task Update(PlayerEmailEntity item);

    }
}
