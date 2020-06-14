using AutoMapper;
using Emprise.Application.Config.Dtos;
using Emprise.Domain.Config.Models;
using Emprise.Domain.Config.Services;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Services;
using Emprise.Domain.Log.Entity;
using Emprise.Domain.Log.Services;
using Emprise.Infra.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Application.Config.Services
{
    public class ConfigAppService : BaseAppService, IConfigAppService
    {
        private readonly IMapper _mapper;
        private readonly IConfigDomainService _configDomainService;
        private readonly IOperatorLogDomainService _operatorLogDomainService;
        public ConfigAppService(
            IMapper mapper,
            IConfigDomainService configDomainService,
            IUnitOfWork uow,
            IOperatorLogDomainService operatorLogDomainService)
            : base(uow)
        {
            _mapper = mapper;
            _configDomainService = configDomainService;
            _operatorLogDomainService = operatorLogDomainService;
        }

        public async Task<List<ConfigModel>> GetConfigs()
        {
            var oldConfigDic = await _configDomainService.GetConfigsFromDb();

            return await _configDomainService.FormatConfigs(oldConfigDic);
        }

        public async Task<ResultDto> UpdateConfigs(Dictionary<string, string> configDic)
        {
            var result = new ResultDto { Message = "" };
            try
            {
                var oldConfigDic = await _configDomainService.GetConfigsFromDb();
                var content = oldConfigDic.ComparisonTo(configDic);
                await _configDomainService.UpdateConfigs(configDic, oldConfigDic);

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.修改配置,
                    Content = content
                });

                await Commit();
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                await _operatorLogDomainService.AddError(new OperatorLogEntity
                {
                    Type = OperatorLogType.修改配置,
                    Content = $"Data={JsonConvert.SerializeObject(configDic)},ErrorMessage={result.Message}"
                });
                await Commit();
            }
            return result;
        }
    }
}
