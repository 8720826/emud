
using Emprise.Application.Ware.Dtos;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Log.Entity;
using Emprise.Domain.Ware.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Application.Ware.Services
{
    public interface IWareAppService : IBaseService
    {
        Task<WareEntity> Get(int id);

        Task<ResultDto> Add(WareInput item);

        Task<ResultDto> Update(int id, WareInput item);

        Task<ResultDto> Delete(int id);

        Task<Paging<WareEntity>> GetPaging(string keyword, int pageIndex);
    }
}
