using Emprise.Application.Config.Dtos;
using Emprise.Domain.Config.Models;
using Emprise.Domain.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Application.Config.Services
{
    public interface IConfigAppService : IBaseService
    {
        Task<List<ConfigModel>> GetConfigs();

        Task<ResultDto> UpdateConfigs(Dictionary<string, string> configDic);
    }
}
