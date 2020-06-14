using Emprise.Domain.Core.Entity;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.User.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.User.Services
{
    public interface IUserDomainService : IBaseService
    {
        Task<IQueryable<UserEntity>> GetAll();
        Task<UserEntity> Get(Expression<Func<UserEntity, bool>> where);

        Task<UserEntity> Get(int id);

        Task Add(UserEntity user);

        Task Update(UserEntity user);
    }
}
