using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Interfaces;
using Emprise.Infra.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Emprise.Domain.Core.Queue.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Emprise.Infra.Bus
{
    /// <summary>
    /// 重复性队列
    /// 
    /// </summary>
    public class RecurringQueue : IRecurringQueue
    {
        private readonly string queueName = "recurringqueue";
        private readonly IRedisDb _redisDb;
        private readonly IMemoryCache _cache;
        private readonly ILogger<RecurringQueue> _logger;


        public RecurringQueue(IRedisDb redisDb, IMemoryCache cache, ILogger<RecurringQueue> logger)
        {
            _redisDb = redisDb;
            _cache = cache;
            _logger = logger;
        }

        public async Task<bool> Publish<T>(int playerId, T t, int delayMin, int delayMax = 0)
        {
            if (delayMin <= 0)
            {
                return await Task.FromResult(false);
            }
            var channel = t.GetType().Name.ToLower();
            var key = $"{queueName}_{channel}";
            Random rnd = new Random();
            var delay = delayMax > delayMin ? rnd.Next(delayMin, delayMax) : delayMin;
            var message = new QueueData<T>
            {
                DelayMin = delayMin,
                DelayMax = delayMax,
                Data = t,
                DelayTime = DateTime.Now.AddSeconds(delay)
            };

            var isSuccess = await _redisDb.HashSet(key, playerId.ToString(), message);
            if (isSuccess)
            {
                await RemoveCache(channel);
            }
            return isSuccess;
        }


        public async Task<Dictionary<int,T>> Subscribe<T>()
        {
            var list = new Dictionary<int, T>();
            var channel = typeof(T).Name.ToLower();
            var key = $"{queueName}_{channel}";
            var dic = await _redisDb.HashGetAll<QueueData<T>>(key);
            //var dic = await GetAll(channel);
            if (dic == null || dic.Count == 0)
            {
             
                return default;
            }

            bool hasChange = false;
            Random rnd = new Random();
            foreach (var item in dic)
            {
                var itemKey = item.Key;
                var itemValue = item.Value;
                if (itemValue.DelayTime.Subtract(DateTime.Now).TotalMilliseconds > 0)
                {
                    //没到时间
                    continue;
                }

                var delay = itemValue.DelayMax > itemValue.DelayMin ? rnd.Next(itemValue.DelayMin, itemValue.DelayMax) : itemValue.DelayMin;

                //下次消费时间
                itemValue.DelayTime = DateTime.Now.AddSeconds(delay);

                int.TryParse(itemKey, out int playerId);
                if (!list.ContainsKey(playerId))
                {
                    list.Add(playerId, itemValue.Data);
                }
                await _redisDb.HashSet($"{queueName}_{channel}", itemKey, itemValue);
                hasChange = true;
            }

            if (hasChange)
            {
                await RemoveCache(channel);
            }

            return list;
        }

        private async Task<Dictionary<string, QueueData<T>>> GetAll<T>(string channel)
        {
            var key = $"{queueName}_{channel}";
            return await _redisDb.HashGetAll<QueueData<T>>(key);
            /*
            return await _cache.GetOrCreateAsync(key, async q => {
                return await Task.Run(async () =>
                {
                    return await _redisDb.HashGetAll<QueueData>(key);
                });
            });*/
        }


        private async Task RemoveCache(string channel)
        {
            /*
            var key = $"{queueName}_{channel}";
            await Task.Run(() => {
                _cache.Remove(key);
            });
            */
        }

        public async Task Remove<T>(int playerId)
        {
            var channel = typeof(T).Name.ToLower();
            var key = $"{queueName}_{channel}";
            
            await _redisDb.HashDelete(key, playerId.ToString());
            await RemoveCache(channel);

        }

    }
}
