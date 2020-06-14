using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.ItemDrop.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.ItemDrop.Services
{
    public interface IItemDropDomainService : IBaseService
    {
        Task<ItemDropEntity> Get(Expression<Func<ItemDropEntity, bool>> where);



        Task<IQueryable<ItemDropEntity>> GetAll();

        Task<ItemDropEntity> Get(int id);

        Task Add(ItemDropEntity entity);

        Task Update(ItemDropEntity entity);

        Task Delete(ItemDropEntity entity);
    }
}
