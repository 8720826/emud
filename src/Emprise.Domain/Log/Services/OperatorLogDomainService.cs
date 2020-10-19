using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Services;
using Emprise.Domain.Log.Entity;
using Emprise.Infra.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Log.Services
{
    public class OperatorLogDomainService: BaseDomainService<OperatorLogEntity>, IOperatorLogDomainService
    {
        private readonly IRepository<OperatorLogEntity> _logRepository;
        protected readonly IHttpContextAccessor _httpAccessor;
        public OperatorLogDomainService(IRepository<OperatorLogEntity> logRepository, IHttpContextAccessor httpAccessor, IMemoryCache cache, IMediatorHandler bus) : base(logRepository, cache, bus)
        {
            _logRepository = logRepository;
            _httpAccessor = httpAccessor;
        }


        public async Task ClearLog(DateTime dt)
        {
            var logs = (await _logRepository.GetAll()).Where(x => x.CreateDate < dt).ToList();
            foreach (var log in logs)
            {
                await _logRepository.Remove(log);
            }
        }

        public async Task AddSuccess(OperatorLogEntity item)
        {

            item.IsSuccess = true;
            item.Ip = _httpAccessor.HttpContext.GetUserIp();
            item.AdminName = item.AdminName ?? _httpAccessor.HttpContext.User.Identity.Name;
            item.CreateDate = DateTime.Now;

            await _logRepository.Add(item);
        }

        public async Task AddError(OperatorLogEntity item)
        {
            item.IsSuccess = false;
            item.Ip = _httpAccessor.HttpContext.GetUserIp();
            item.AdminName = item.AdminName ?? _httpAccessor.HttpContext.User.Identity.Name;
            item.CreateDate = DateTime.Now;

            await _logRepository.Add(item);
        }

    }
}
