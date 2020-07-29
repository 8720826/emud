using AutoMapper;
using Emprise.Application.ItemDrop.Dtos;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Services;
using Emprise.Domain.ItemDrop.Entity;
using Emprise.Domain.ItemDrop.Services;
using Emprise.Domain.Log.Entity;
using Emprise.Domain.Log.Services;
using Emprise.Infra.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Application.ItemDrop.Services
{
    public class ItemDropRateAppService : BaseAppService, IItemDropRateAppService
    {
        private readonly IMapper _mapper;
        private readonly IItemDropRateDomainService _itemDropRateDomainService;
        private readonly IOperatorLogDomainService _operatorLogDomainService;
        public ItemDropRateAppService(
            IMapper mapper,
            IItemDropRateDomainService itemDropRateDomainService,
            IUnitOfWork uow,
            IOperatorLogDomainService operatorLogDomainService)
            : base(uow)
        {
            _mapper = mapper;
            _itemDropRateDomainService = itemDropRateDomainService;
            _operatorLogDomainService = operatorLogDomainService;
        }


        public async Task<List<ItemDropRateEntity>> GetAll(int id)
        {
            var query = await _itemDropRateDomainService.GetAll();

            return query.Where(x => x.ItemDropId == id).ToList();
        }

        public async Task<ItemDropRateEntity> Get(int id)
        {
            return await _itemDropRateDomainService.Get(id);

        }

        public async Task<ResultDto> Add(int id, ItemDropRateInput input)
        {

            var result = new ResultDto { Message = "" };

            try
            {
                var itemDropRate = _mapper.Map<ItemDropRateEntity>(input);

                itemDropRate.ItemDropId = id;
                await _itemDropRateDomainService.Add(itemDropRate);

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.添加掉落项,
                    Content = JsonConvert.SerializeObject(input)
                });

                await Commit();
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                await _operatorLogDomainService.AddError(new OperatorLogEntity
                {
                    Type = OperatorLogType.添加掉落项,
                    Content = $"Data={JsonConvert.SerializeObject(input)},ErrorMessage={result.Message}"
                });
                await Commit();
            }
            return result;
        }

        public async Task<ResultDto> Update(int id, ItemDropRateInput input)
        {

            var result = new ResultDto { Message = "" };
            try
            {
                var rate = await _itemDropRateDomainService.Get(id);
                if (rate == null)
                {
                    result.Message = $"掉落项 {id} 不存在！";
                    return result;
                }

                var content = rate.ComparisonTo(input);
                _mapper.Map(input, rate);

                await _itemDropRateDomainService.Update(rate);

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.修改掉落项,
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
                    Type = OperatorLogType.修改掉落项,
                    Content = $"Data={JsonConvert.SerializeObject(input)},ErrorMessage={result.Message}"
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
                var rate = await _itemDropRateDomainService.Get(id);
                if (rate == null)
                {
                    result.Message = $"掉落项 {id} 不存在！";
                    return result;
                }


                await _itemDropRateDomainService.Delete(rate);

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.删除掉落项,
                    Content = JsonConvert.SerializeObject(rate)
                });

                await Commit();

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                await _operatorLogDomainService.AddError(new OperatorLogEntity
                {
                    Type = OperatorLogType.删除掉落项,
                    Content = $"id={id}，ErrorMessage={result.Message}"
                });
                await Commit();
            }
            return result;
        }
    }
}
