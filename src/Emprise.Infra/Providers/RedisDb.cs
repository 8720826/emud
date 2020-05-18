using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Infra.Providers
{
    public class RedisDb: IRedisDb
    {
        private readonly IDatabase db;
        private readonly AppConfig _appConfig;
        private readonly ISubscriber sub;
        private readonly ILogger<RedisDb> _logger;
        private readonly IConfiguration _configuration;

        public RedisDb(IOptionsMonitor<AppConfig> appConfig, ILogger<RedisDb> logger, IConfiguration configuration)
        {
            _appConfig = appConfig.CurrentValue;
            _logger = logger;
            _configuration = configuration;
            var redis = ConnectionMultiplexer.Connect(_configuration.GetValue<string>("Redis"));
            db = redis.GetDatabase();
            sub = redis.GetSubscriber();
        }


        public async Task<bool> StringSet<T>(string key, T t, DateTime? expiry = null)
        {
            var value = JsonConvert.SerializeObject(t);
            var result = await db.StringSetAsync(key, value);

            if (expiry.HasValue)
            {
                await KeyExpire(key, expiry.Value);
            }

            return result;
        }

        public async Task<T> StringGet<T>(string key)
        {
            try 
            {
                var value = await db.StringGetAsync(key);
                if (!string.IsNullOrEmpty(value))
                {
                    return JsonConvert.DeserializeObject<T>(value);
                }
            }
            catch(Exception ex)
            {
                //TODO
            }
            return default;
        }

        public async Task KeyExpire(string key, DateTime expiry)
        {
            await db.KeyExpireAsync(key, expiry);
        }

        public async Task<long> KeyTimeToLive(string key)
        {
            var ts = await db.KeyTimeToLiveAsync(key);
            return ts.HasValue ? ts.Value.Ticks : -1;
        }

        public async Task<bool> KeyDelete(string key)
        {
            return await db.KeyDeleteAsync(key);
        }

        public async Task<Dictionary<string,T>> HashGetAll<T>(string key)
        {
            var arr = await db.HashGetAllAsync(key);
           // _logger.LogDebug($"key={key},arr={Newtonsoft.Json.JsonConvert.SerializeObject(arr)}");
            var dic = new Dictionary<string, T>();

            try
            {
                foreach (var item in arr)
                {
                    if (!item.Value.IsNullOrEmpty && !dic.ContainsKey(item.Name))
                    {
                        dic.Add(item.Name, JsonConvert.DeserializeObject<T>(item.Value));
                    }
                }
            }
            catch (Exception ex) {
                //_logger.LogError($"Exception={ex}");
            }

            return dic;
        }

        public async Task<T> HashGet<T>(string key, string field)
        {
            var item = await db.HashGetAsync(key, field);
            if (!item.IsNullOrEmpty)
            {
                return JsonConvert.DeserializeObject<T>(item);
            }
            return default;
        }

        public async Task<bool> HashDelete(string key, string field)
        {
            return await db.HashDeleteAsync(key, field);
        }

        public async Task<bool> HashSet<T>(string key, string field, T t)
        {
            var value = JsonConvert.SerializeObject(t);
            return await db.HashSetAsync(key, field, value);
        }


        public async Task<bool> SortedSetAdd(string key, string value, double score)
        {
            return await db.SortedSetAddAsync(key, value, score);
        }

        public async Task<bool> SortedSetRemove(string key, string value)
        {
            return await db.SortedSetRemoveAsync(key, value);
        }

        public async Task<List<string>> SortedSetRangeByScore(string key, double score)
        {
            var items = await db.SortedSetRangeByScoreAsync(key, 0, score);
            if (items == null || items.Length == 0)
            {
                return default;
            }
            List<string> list = new List<string>();
            return items.Where(x=>x.HasValue).Select(x => x.ToString()).ToList();
        }

        public async Task<long> Publish(string channel, string message)
        {
            return await sub.PublishAsync(channel, message);
        }

        public async Task Subscribe(string channel, Action<string, string>  action)
        {
            await sub.SubscribeAsync(channel,  (channel, message) =>
            {
                action(channel, message);

            });
        }
    }
}
