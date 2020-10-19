using Emprise.Domain.Core.Interfaces.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Core.Interfaces
{
    public interface IBaseDomainService<TEntity> : IScoped where TEntity : class
    {
        Task<IQueryable<TEntity>> GetAll();

        Task<IQueryable<TEntity>> GetAllFromCache();

        Task<TEntity> Get(Expression<Func<TEntity, bool>> where);

        Task<TEntity> Get(int id);

        Task<TEntity> GetFromCache(int id);

        Task Add(TEntity t);

        Task Update(TEntity t);

        Task Delete(TEntity t);
    }
}
