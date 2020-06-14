using Emprise.Domain.Admin.Entity;
using Emprise.Domain.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Admin.Services
{
    public interface IAdminDomainService : IBaseService
    {
        Task<IQueryable<AdminEntity>> GetAll();

        Task<AdminEntity> Get(Expression<Func<AdminEntity, bool>> where);

        Task Add(AdminEntity entity);

        Task Update(AdminEntity entity);

        Task Delete(AdminEntity entity);
    }
}
