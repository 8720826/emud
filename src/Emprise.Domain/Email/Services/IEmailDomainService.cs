using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Email.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Email.Services
{
    public interface IEmailDomainService : IBaseDomainService<EmailEntity>
    {

        Task<List<EmailEntity>> GetMyEmails(int playerId, int factionId=0);

    }
}
