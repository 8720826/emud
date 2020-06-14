using Emprise.Application.ItemDrop.Dtos;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.ItemDrop.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Application.ItemDrop.Services
{
    public interface IItemDropAppService : IBaseService
    {
        Task<ItemDropEntity> Get(int id);

        Task<ResultDto> Add(ItemDropInput item);

        Task<ResultDto> Update(int id, ItemDropInput item);

        Task<ResultDto> Delete(int id);

        Task<Paging<ItemDropEntity>> GetPaging(string keyword, int pageIndex);
    }
}
