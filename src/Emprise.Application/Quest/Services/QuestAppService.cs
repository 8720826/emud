using AutoMapper;
using Emprise.Application.Quest.Dtos;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Services;
using Emprise.Domain.Log.Entity;
using Emprise.Domain.Log.Services;
using Emprise.Domain.Quest.Entity;
using Emprise.Domain.Quest.Services;
using Emprise.Infra.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Application.Quest.Services
{
    public class QuestAppService : BaseAppService, IQuestAppService
    {
        private readonly IMapper _mapper;
        private readonly IQuestDomainService _questDomainService;
        private readonly IOperatorLogDomainService _operatorLogDomainService;
        public QuestAppService(
            IMapper mapper,
            IQuestDomainService questDomainService,
            IUnitOfWork uow,
            IOperatorLogDomainService operatorLogDomainService)
            : base(uow)
        {
            _mapper = mapper;
            _questDomainService = questDomainService;
            _operatorLogDomainService = operatorLogDomainService;
        }

        public async Task<QuestEntity> Get(int id)
        {
            return await _questDomainService.Get(id);
        }

        public async Task<ResultDto> Add(QuestInput item)
        {
            var result = new ResultDto { Message = "" };

            try
            {
                var itemDrop = _mapper.Map<QuestEntity>(item);


                await _questDomainService.Add(itemDrop);

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.添加任务,
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
                    Type = OperatorLogType.添加任务,
                    Content = $"Data={JsonConvert.SerializeObject(item)},ErrorMessage={result.Message}"
                });
                await Commit();
            }
            return result;
        }

        public async Task<ResultDto> Update(int id, QuestInput item)
        {
            var result = new ResultDto { Message = "" };
            try
            {
                var map = await _questDomainService.Get(id);
                if (map == null)
                {
                    result.Message = $"任务 {id} 不存在！";
                    return result;
                }

                var content = map.ComparisonTo(item);
                _mapper.Map(item, map);

                await _questDomainService.Update(map);

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.修改任务,
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
                    Type = OperatorLogType.修改任务,
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
                var map = await _questDomainService.Get(id);
                if (map == null)
                {
                    result.Message = $"任务 {id} 不存在！";
                    return result;
                }

                await _questDomainService.Delete(map);

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.删除任务,
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
                    Type = OperatorLogType.删除任务,
                    Content = $"id={id}，ErrorMessage={result.Message}"
                });
                await Commit();
            }
            return result;
        }

        public async Task<Paging<QuestEntity>> GetPaging(string keyword, int pageIndex)
        {

            var query = await _questDomainService.GetAll();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Name.Contains(keyword));
            }
            query = query.OrderBy(x => x.Id);

            return await query.Paged(pageIndex);
        }
    }
}
