using AutoMapper;
using Emprise.Application.Map.Dtos;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Services;
using Emprise.Domain.Log.Entity;
using Emprise.Domain.Log.Services;
using Emprise.Domain.Map.Entity;
using Emprise.Domain.Map.Services;
using Emprise.Infra.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Application.Map.Services
{
    public class MapAppService: BaseAppService, IMapAppService
    {
        private readonly IMapper _mapper;
        private readonly IMapDomainService _mapDomainService;
        private readonly IOperatorLogDomainService _operatorLogDomainService;
        public MapAppService(
            IMapper mapper,
            IMapDomainService mapDomainService,
            IUnitOfWork uow,
            IOperatorLogDomainService operatorLogDomainService)
            : base(uow)
        {
            _mapper = mapper;
            _mapDomainService = mapDomainService;
            _operatorLogDomainService = operatorLogDomainService;
        }

        public async Task<MapEntity> Get(int id)
        {
            return await _mapDomainService.Get(id);
        }

        public async Task<ResultDto> Add(MapInput item)
        {
            var result = new ResultDto { Message = "" };

            try
            {
                var map = _mapper.Map<MapEntity>(item);


                await _mapDomainService.Add(map);

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.添加地图,
                    Content = JsonConvert.SerializeObject(item)
                });

                await Commit();
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                await _operatorLogDomainService.AddError(new OperatorLogEntity
                {
                    Type = OperatorLogType.添加地图,
                    Content = $"Data={JsonConvert.SerializeObject(item)},ErrorMessage={result.Message}"
                });
                await Commit();
            }
            return result;
        }

        public async Task<ResultDto> Update(int id, MapInput item)
        {
            var result = new ResultDto { Message = "" };
            try
            {
                var map = await _mapDomainService.Get(id);
                if (map == null)
                {
                    result.Message = $"地图 {id} 不存在！";
                    return result;
                }

                var content = map.ComparisonTo(item);
                _mapper.Map(item, map);

                await _mapDomainService.Update(map);

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.修改物品,
                    Content = $"Id = {id},Data = {content}"
                });

                await Commit();

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                await _operatorLogDomainService.AddError(new OperatorLogEntity
                {
                    Type = OperatorLogType.修改地图,
                    Content = $"Data={JsonConvert.SerializeObject(item)},ErrorMessage={result.Message}"
                });
                await Commit();
            }
            return result;
        }

        public async Task<ResultDto> Delete(int id)
        {
            var result = new ResultDto { Message = "" };
            try
            {
                var map = await _mapDomainService.Get(id);
                if (map == null)
                {
                    result.Message = $"地图 {id} 不存在！";
                    return result;
                }

                var roomCount = await _mapDomainService.GetRoomCount(id);
                if (roomCount > 0)
                {
                    result.Message = $"该地图下还有{roomCount}个房间，删除失败";

                    return result;
                }



                await _mapDomainService.Delete(map);

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.删除地图,
                    Content = JsonConvert.SerializeObject(map)
                });

                await Commit();

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                await _operatorLogDomainService.AddError(new OperatorLogEntity
                {
                    Type = OperatorLogType.删除地图,
                    Content = $"id={id}，ErrorMessage={result.Message}"
                });
                await Commit();
            }
            return result;
        }

        public async Task<Paging<MapEntity>> GetPaging(string keyword, int pageIndex)
        {

            var query = await _mapDomainService.GetAll();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Name.Contains(keyword));
            }
            query = query.OrderBy(x => x.Id);

            return await query.Paged(pageIndex);
        }

    }
}
