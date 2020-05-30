using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Email.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Email.Services
{
    public interface IEmailDomainService : IBaseService
    {
        Task<IQueryable<EmailEntity>> Query();

        Task<List<EmailEntity>> GetMyEmails(int playerId, int factionId=0);

    }
}
