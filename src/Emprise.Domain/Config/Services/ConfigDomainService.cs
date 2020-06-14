using Emprise.Domain.Config.Models;
using Emprise.Domain.Core.Models;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Config.Services
{
    public class ConfigDomainService: IConfigDomainService
    {
        private readonly IConfiguration _configuration;
        private IDatabase _redisDb;
        public ConfigDomainService(IConfiguration configuration)
        {
            _configuration = configuration;
            var redis = ConnectionMultiplexer.Connect(_configuration.GetValue<string>("Redis"));
            _redisDb = redis.GetDatabase();
        }


        public async Task<Dictionary<string, string>> GetConfigsFromDb()
        {
            return await Task.Run(() =>
            {
                var configurations = _redisDb.HashGetAll("configurations");
                return configurations.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());
            });

        }

        public async Task<List<ConfigModel>> FormatConfigs(Dictionary<string, string> configDic)
        {
            return await Task.Run(() =>
            {
                return GetAppConfigValue(configDic, typeof(AppConfig));
            });
           
        }


        public async Task UpdateConfigs(Dictionary<string, string> configDic, Dictionary<string, string> oldConfigDic)
        {
            var configs = GetAppConfigValue(configDic, typeof(AppConfig));
            foreach (var config in configs)
            {
                if (!oldConfigDic.ContainsKey(config.Key))
                {
                    _redisDb.HashSet("configurations", config.Key, config.Value);
                }
                else if (oldConfigDic[config.Key] != config.Value)
                {
                    _redisDb.HashSet("configurations", config.Key, config.Value);
                }
            }
        }


        private List<ConfigModel> GetAppConfigValue(Dictionary<string,string> configDic, Type type, string parentName = "")
        {
            var configs = new List<ConfigModel>();
            PropertyInfo[] props = type.GetProperties();
            foreach (PropertyInfo prop in props)
            {

                string name = prop.Name;
                string key = string.IsNullOrEmpty(parentName) ? $"{name}" : $"{parentName}:{name}";

                if (prop.PropertyType.IsValueType || prop.PropertyType.Name.StartsWith("String"))
                {
                    configDic.TryGetValue(key, out string value);
                    var attribute = prop.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute;
                    configs.Add(new ConfigModel
                    {
                        Key = key,
                        Name = attribute?.DisplayName ?? name,
                        Value = value,
                        Type = prop.PropertyType
                    });
                }
                else
                {
                    configs.AddRange(GetAppConfigValue(configDic, prop.PropertyType, key));
                }

            }

            return configs;
        }

    }
}
