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
    public interface IEmailDomainService : IBaseService
    {
        Task<EmailEntity> Get(Expression<Func<EmailEntity, bool>> where);



        Task<IQueryable<EmailEntity>> GetAll();

        Task<EmailEntity> Get(int id);

        Task Add(EmailEntity entity);

        Task Update(EmailEntity entity);

        Task Delete(EmailEntity entity);

        Task<List<EmailEntity>> GetMyEmails(int playerId, int factionId=0);

    }
}
