using AutoMapper;
using Emprise.Application.Email.Dtos;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Services;
using Emprise.Domain.Email.Entity;
using Emprise.Domain.Email.Services;
using Emprise.Domain.Log.Entity;
using Emprise.Domain.Log.Services;
using Emprise.Infra.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Application.Email.Services
{
    public class EmailAppService : BaseAppService, IEmailAppService
    {
        private readonly IMapper _mapper;
        private readonly IEmailDomainService _emailDomainService;
        private readonly IOperatorLogDomainService _operatorLogDomainService;
        public EmailAppService(
            IMapper mapper,
            IEmailDomainService emailDomainService,
            IUnitOfWork uow,
            IOperatorLogDomainService operatorLogDomainService)
            : base(uow)
        {
            _mapper = mapper;
            _emailDomainService = emailDomainService;
            _operatorLogDomainService = operatorLogDomainService;
        }

        public async Task<EmailEntity> Get(int id)
        {
            return await _emailDomainService.Get(id);
        }

        public async Task<ResultDto> Add(EmailInput item)
        {
            var result = new ResultDto { Message = "" };

            try
            {
                var email = _mapper.Map<EmailEntity>(item);
                await _emailDomainService.Add(email);

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.发送邮件,
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
                    Type = OperatorLogType.发送邮件,
                    Content = $"Data={JsonConvert.SerializeObject(item)},ErrorMessage={result.Message}"
                });
                await Commit();
            }
            return result;
        }

        public async Task<ResultDto> Update(int id, EmailInput item)
        {
            var result = new ResultDto { Message = "" };
            try
            {
                var email = await _emailDomainService.Get(id);
                if (email == null)
                {
                    result.Message = $"邮件 {id} 不存在！";
                    return result;
                }
                var content = email.ComparisonTo(item);
                _mapper.Map(item, email);

                await _emailDomainService.Update(email);

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.修改邮件,
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
                    Type = OperatorLogType.修改邮件,
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
                var email = await _emailDomainService.Get(id);
                if (email == null)
                {
                    result.Message = $"邮件 {id} 不存在！";
                    return result;
                }


                await _emailDomainService.Delete(email);

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.删除邮件,
                    Content = JsonConvert.SerializeObject(email)
                });

                await Commit();

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                await _operatorLogDomainService.AddError(new OperatorLogEntity
                {
                    Type = OperatorLogType.删除邮件,
                    Content = $"id={id}，ErrorMessage={result.Message}"
                });
                await Commit();
            }
            return result;
        }

        public async Task<Paging<EmailEntity>> GetPaging(string keyword, int pageIndex)
        {

            var query = await _emailDomainService.GetAll();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Title.Contains(keyword)|| x.Content.Contains(keyword));
            }
            query = query.OrderBy(x => x.Id);

            return await query.Paged(pageIndex);
        }
    }
}
