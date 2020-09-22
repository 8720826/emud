using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Core.Services
{
    public class BaseDomainService<TEntity>  where TEntity : class
    {
        private readonly IRepository<TEntity> _repository;

        public BaseDomainService(IRepository<TEntity> repository)
        {
            _repository = repository;
        }

        public async Task<IQueryable<TEntity>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<TEntity> Get(Expression<Func<TEntity, bool>> where)
        {
            return await _repository.Get(where);
        }

        public async Task<TEntity> Get(int id)
        {
            return await _repository.Get(id);
        }

        public async Task Add(TEntity entity)
        {
            await _repository.Add(entity);
        }

        public async Task Update(TEntity entity)
        {
            await _repository.Update(entity);
        }

        public async Task Delete(TEntity entity)
        {
            await _repository.Remove(entity);
        }
    }
}
