using AutoMapper;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Services;
using Emprise.Domain.Log.Entity;
using Emprise.Domain.Log.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Application.Log.Services
{

    public class OperatorLogAppService : BaseAppService, IOperatorLogAppService
    {
        private readonly IMapper _mapper;
        private readonly IOperatorLogDomainService _operatorLogDomainService;
        public OperatorLogAppService(IMapper mapper, IOperatorLogDomainService operatorLogDomainService, IUnitOfWork uow) 
            : base(uow)
        {
            _mapper = mapper;
            _operatorLogDomainService = operatorLogDomainService;
        }

        public async Task ClearLog(DateTime dt)
        {
            await _operatorLogDomainService.ClearLog(dt);
            await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
            {
                Type = OperatorLogType.清除记录,
                Content = $"清除{dt.ToFriendlyTime()}记录"
            });
        }

        public async Task<Paging<OperatorLogEntity>> GetPaging(string keyword, int pageIndex)
        {

            var query = await _operatorLogDomainService.GetAll();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Content.Contains(keyword));
            }
            query = query.OrderBy(x => x.Id);

            return await query.Paged(pageIndex);
        }
    }
}
