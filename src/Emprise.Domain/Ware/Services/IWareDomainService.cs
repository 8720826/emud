using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Ware.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Ware.Services
{

    public interface IWareDomainService : IBaseService
    {
        Task<WareEntity> Get(Expression<Func<WareEntity, bool>> where);



        Task<IQueryable<WareEntity>> GetAll();

        Task<WareEntity> Get(int id);

        Task Add(WareEntity entity);

        Task Update(WareEntity entity);

        Task Delete(WareEntity entity);
    }
}
