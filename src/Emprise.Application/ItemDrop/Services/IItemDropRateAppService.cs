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
    public interface IItemDropRateAppService : IBaseService
    {
        Task<List<ItemDropRateEntity>> GetAll(int id);

        Task<ItemDropRateEntity> Get(int id);

        Task<ResultDto> Update(int id, ItemDropRateInput input);

        Task<ResultDto> Add(int id, ItemDropRateInput input);

        Task<ResultDto> Delete(int id);
    }
}
