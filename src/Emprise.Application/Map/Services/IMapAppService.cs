using Emprise.Application.Map.Dtos;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Map.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Application.Map.Services
{
    public interface IMapAppService : IBaseService
    {
        Task<MapEntity> Get(int id);

        Task<ResultDto> Add(MapInput item);

        Task<ResultDto> Update(int id, MapInput item);

        Task<ResultDto> Delete(int id);

        Task<Paging<MapEntity>> GetPaging(string keyword, int pageIndex);
    }
}
