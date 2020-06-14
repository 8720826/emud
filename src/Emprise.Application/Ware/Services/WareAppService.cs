using AutoMapper;
using Emprise.Application.Ware.Dtos;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Services;
using Emprise.Domain.Log.Entity;
using Emprise.Domain.Log.Services;
using Emprise.Domain.Ware.Entity;
using Emprise.Domain.Ware.Services;
using Emprise.Infra.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Application.Ware.Services
{

    public class WareAppService : BaseAppService, IWareAppService
    {
        private readonly IMapper _mapper;
        private readonly IWareDomainService _wareDomainService;
        private readonly IOperatorLogDomainService _operatorLogDomainService;
        public WareAppService(
            IMapper mapper, 
            IWareDomainService wareDomainService, 
            IUnitOfWork uow, 
            IOperatorLogDomainService operatorLogDomainService) 
            : base(uow)
        {
            _mapper = mapper;
            _wareDomainService = wareDomainService;
            _operatorLogDomainService = operatorLogDomainService;
        }

        public async Task<WareEntity> Get(int id)
        {
            return await _wareDomainService.Get(id);
        }

        public async Task<ResultDto> Add(WareInput item)
        {
            var result = new ResultDto { Message = "" };

            try
            {
                var ware = _mapper.Map<WareEntity>(item);
                if (ware.Effect == null)
                {
                    ware.Effect = "";
                }

                await _wareDomainService.Add(ware);

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.添加物品,
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
                    Type = OperatorLogType.添加物品,
                    Content = $"Data={JsonConvert.SerializeObject(item)},ErrorMessage={result.Message}"
                });
                await Commit();
            }
            return result;
        }

        public async Task<ResultDto> Update(int id, WareInput item)
        {
            var result = new ResultDto { Message = "" };
            try
            {
                var ware = await _wareDomainService.Get(id);
                if (ware == null)
                {
                    result.Message = $"物品 {id} 不存在！";
                    return result;
                }
                var content = ware.ComparisonTo(item);
                _mapper.Map(item, ware);

                if (ware.Effect == null)
                {
                    ware.Effect = "";
                }

                await _wareDomainService.Update(ware);

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
                    Type = OperatorLogType.修改物品,
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
                var ware = await _wareDomainService.Get(id);
                if (ware == null)
                {
                    result.Message = $"物品 {id} 不存在！";
                    return result;
                }


                await _wareDomainService.Delete(ware);

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.删除物品,
                    Content = JsonConvert.SerializeObject(ware)
                });

                await Commit();

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                await _operatorLogDomainService.AddError(new OperatorLogEntity
                {
                    Type = OperatorLogType.删除物品,
                    Content = $"id={id}，ErrorMessage={result.Message}"
                });
                await Commit();
            }
            return result;
        }

        public async Task<Paging<WareEntity>> GetPaging(string keyword,int pageIndex)
        {

            var query = await _wareDomainService.GetAll();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Name.Contains(keyword));
            }
            query= query.OrderBy(x => x.Id);

            return await query.Paged(pageIndex);
        }
    }
}
