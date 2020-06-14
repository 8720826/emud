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
    public class ItemDropAppService : BaseAppService, IItemDropAppService
    {
        private readonly IMapper _mapper;
        private readonly IItemDropDomainService _itemDropDomainService;
        private readonly IOperatorLogDomainService _operatorLogDomainService;
        public ItemDropAppService(
            IMapper mapper,
            IItemDropDomainService itemDropDomainService,
            IUnitOfWork uow,
            IOperatorLogDomainService operatorLogDomainService)
            : base(uow)
        {
            _mapper = mapper;
            _itemDropDomainService = itemDropDomainService;
            _operatorLogDomainService = operatorLogDomainService;
        }

        public async Task<ItemDropEntity> Get(int id)
        {
            return await _itemDropDomainService.Get(id);
        }

        public async Task<ResultDto> Add(ItemDropInput item)
        {
            var result = new ResultDto { Message = "" };

            try
            {
                var itemDrop = _mapper.Map<ItemDropEntity>(item);


                await _itemDropDomainService.Add(itemDrop);

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.添加掉落,
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
                    Type = OperatorLogType.添加掉落,
                    Content = $"Data={JsonConvert.SerializeObject(item)},ErrorMessage={result.Message}"
                });
                await Commit();
            }
            return result;
        }

        public async Task<ResultDto> Update(int id, ItemDropInput item)
        {
            var result = new ResultDto { Message = "" };
            try
            {
                var map = await _itemDropDomainService.Get(id);
                if (map == null)
                {
                    result.Message = $"掉落 {id} 不存在！";
                    return result;
                }

                var content = map.ComparisonTo(item);
                _mapper.Map(item, map);

                await _itemDropDomainService.Update(map);

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.修改掉落,
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
                    Type = OperatorLogType.修改掉落,
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
                var map = await _itemDropDomainService.Get(id);
                if (map == null)
                {
                    result.Message = $"掉落 {id} 不存在！";
                    return result;
                }

                await _itemDropDomainService.Delete(map);

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.删除掉落,
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
                    Type = OperatorLogType.删除掉落,
                    Content = $"id={id}，ErrorMessage={result.Message}"
                });
                await Commit();
            }
            return result;
        }

        public async Task<Paging<ItemDropEntity>> GetPaging(string keyword, int pageIndex)
        {

            var query = await _itemDropDomainService.GetAll();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Name.Contains(keyword));
            }
            query = query.OrderBy(x => x.Id);

            return await query.Paged(pageIndex);
        }
    }
}
