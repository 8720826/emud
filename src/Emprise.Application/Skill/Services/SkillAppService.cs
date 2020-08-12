using AutoMapper;
using Emprise.Application.Skill.Dtos;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Services;
using Emprise.Domain.Log.Entity;
using Emprise.Domain.Log.Services;
using Emprise.Domain.Skill.Entity;
using Emprise.Domain.Skill.Services;
using Emprise.Infra.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Application.Skill.Services
{
    public class SkillAppService: BaseAppService, ISkillAppService
    {
        private readonly IMapper _mapper;
        private readonly ISkillDomainService _skillDomainService;
        private readonly IOperatorLogDomainService _operatorLogDomainService;
        public SkillAppService(
            IMapper mapper,
            ISkillDomainService skillDomainService,
            IUnitOfWork uow,
            IOperatorLogDomainService operatorLogDomainService)
            : base(uow)
        {
            _mapper = mapper;
            _skillDomainService = skillDomainService;
            _operatorLogDomainService = operatorLogDomainService;
        }

        public async Task<SkillEntity> Get(int id)
        {
            return await _skillDomainService.Get(id);
        }

        public async Task<ResultDto> Add(SkillInput item)
        {
            var result = new ResultDto { Message = "" };

            try
            {
                var ware = _mapper.Map<SkillEntity>(item);


                await _skillDomainService.Add(ware);

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.添加武功,
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
                    Type = OperatorLogType.添加武功,
                    Content = $"Data={JsonConvert.SerializeObject(item)},ErrorMessage={result.Message}"
                });
                await Commit();
            }
            return result;
        }

        public async Task<ResultDto> Update(int id, SkillInput item)
        {
            var result = new ResultDto { Message = "" };
            try
            {
                var ware = await _skillDomainService.Get(id);
                if (ware == null)
                {
                    result.Message = $"物品 {id} 不存在！";
                    return result;
                }
                var content = ware.ComparisonTo(item);
                _mapper.Map(item, ware);



                await _skillDomainService.Update(ware);

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.修改武功,
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
                    Type = OperatorLogType.修改武功,
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
                var skill = await _skillDomainService.Get(id);
                if (skill == null)
                {
                    result.Message = $"武功 {id} 不存在！";
                    return result;
                }


                await _skillDomainService.Delete(skill);

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.删除武功,
                    Content = JsonConvert.SerializeObject(skill)
                });

                await Commit();

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                await _operatorLogDomainService.AddError(new OperatorLogEntity
                {
                    Type = OperatorLogType.删除武功,
                    Content = $"id={id}，ErrorMessage={result.Message}"
                });
                await Commit();
            }
            return result;
        }

        public async Task<Paging<SkillEntity>> GetPaging(string keyword, int pageIndex)
        {

            var query = await _skillDomainService.GetAll();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Name.Contains(keyword));
            }
            query = query.OrderBy(x => x.Id);

            return await query.Paged(pageIndex);
        }
    }
}
