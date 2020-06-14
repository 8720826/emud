using Emprise.Domain.Config.Models;
using Emprise.Domain.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Config.Services
{
    public interface IConfigDomainService : IBaseService
    {
        Task<Dictionary<string, string>> GetConfigsFromDb();

        Task<List<ConfigModel>> FormatConfigs(Dictionary<string, string> configDic);


        Task UpdateConfigs(Dictionary<string, string> configDic, Dictionary<string, string> oldConfigDic);
    }
}
