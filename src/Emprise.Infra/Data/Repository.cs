using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Emprise.Domain.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace Emprise.Infra.Data
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly EmpriseDbContext _db;
        protected readonly DbSet<TEntity> _dbSet;

        public Repository(EmpriseDbContext context)
        {
            _db = context;
            _dbSet = _db.Set<TEntity>();
        }

        public virtual async Task Add(TEntity entity, bool saveChanges = false)
        {
            await _dbSet.AddAsync(entity);

            if (saveChanges)
            {
                await SaveChanges();
            }
        }

        public virtual async Task Add(IEnumerable<TEntity> entities, bool saveChanges = false)
        {
            foreach (var entity in entities)
            {
                await _dbSet.AddAsync(entity);
            }


            if (saveChanges)
            {
                await SaveChanges();
            }
        }

        public virtual async Task<TEntity> Get(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<TEntity> Get(Expression<Func<TEntity, bool>> where)
        {
            return await _dbSet.FirstOrDefaultAsync(where);
        }

        public virtual async Task<IQueryable<TEntity>> GetAll()
        {
            return await Task.Run(() => {
                return _dbSet;
            });
        }

        public virtual async Task<IQueryable<TEntity>> GetAll(Expression<Func<TEntity, bool>> where)
        {
            return await Task.Run(() => {
                return _dbSet.Where(where);
            });
        }

        public virtual async Task Update(TEntity obj, bool saveChanges = false)
        {
            await Task.Run(() => {
                _dbSet.Update(obj);
            });


            if (saveChanges)
            {
                await SaveChanges();
            }
        }

        public virtual async Task Remove(int id, bool saveChanges = false)
        {
            _dbSet.Remove(await _dbSet.FindAsync(id));


            if (saveChanges)
            {
                await SaveChanges();
            }
        }

        public virtual async Task Remove(TEntity obj, bool saveChanges = false)
        {
            await Task.Run(() => {
                _dbSet.Remove(obj);
            });


            if (saveChanges)
            {
                await SaveChanges();
            }
        }

        public async Task<int> SaveChanges()
        {
            return await _db.SaveChangesAsync();
        }

        
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
