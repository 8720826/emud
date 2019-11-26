using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Emprise.Domain.Core.Data
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task Add(TEntity obj, bool saveChanges = true);


        Task<TEntity> Get(int id);

        Task<TEntity> Get(Expression<Func<TEntity, bool>> where);

        Task<IQueryable<TEntity>> GetAll();

        Task<IQueryable<TEntity>> GetAll(Expression<Func<TEntity, bool>> where);

        Task Update(TEntity obj, bool saveChanges = true);


        Task Remove(int id, bool saveChanges = true);


        Task Remove(TEntity obj, bool saveChanges = true);

        //Task<int> SaveChanges();
    }


}
