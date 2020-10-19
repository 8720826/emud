using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Email.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Email.Services
{
    public interface IPlayerEmailDomainService : IBaseDomainService<PlayerEmailEntity>
    {

        Task<List<PlayerEmailEntity>> GetMyReceivedEmails(int playerId);


        Task<int> GetUnreadCount(int playerId);


    }
}
