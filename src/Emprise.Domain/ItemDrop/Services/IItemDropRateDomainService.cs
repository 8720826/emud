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
    public interface IItemDropRateDomainService : IBaseService
    {
        Task<ItemDropRateEntity> Get(Expression<Func<ItemDropRateEntity, bool>> where);



        Task<IQueryable<ItemDropRateEntity>> GetAll();

        Task<ItemDropRateEntity> Get(int id);

        Task Add(ItemDropRateEntity entity);

        Task Update(ItemDropRateEntity entity);

        Task Delete(ItemDropRateEntity entity);
    }
}
