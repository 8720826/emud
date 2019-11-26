using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Queue.Models;
using Emprise.Infra.Queue;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Infra.Bus
{
    public class DelayedQueue : IDelayedQueue
    {
        private readonly string queueName = "delayedqueue";
        private readonly IRedisDb _redisDb;
        public DelayedQueue(IRedisDb redisDb)
        {
            _redisDb = redisDb;
        }

        public async Task<bool> Publish<T>(int playerId, T t, int delayMin, int delayMax = 0)
        {
            var channel = t.GetType().Name.ToLower();

            Random rnd = new Random();
            var delay = delayMax > delayMin ? rnd.Next(delayMin, delayMax) : delayMin;
            var timestamp = DateTimeOffset.Now.AddSeconds(delay).ToUnixTimeSeconds();

            var hasAdd = await _redisDb.SortedSetAdd($"{queueName}_{channel}", playerId.ToString(), timestamp);
            if (hasAdd)
            {
                return await _redisDb.StringSet($"{queueName}_{channel}_{playerId}", t, DateTime.Now.AddSeconds(delay).AddDays(1));
            }
            return await Task.FromResult(false);
        }

        public async Task<List<T>> Subscribe<T>()
        {
            var channel = typeof(T).Name.ToLower();
            var list = new List<T>();
            var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            var playerIds = await _redisDb.SortedSetRangeByScore($"{queueName}_{channel}", timestamp);
            if (playerIds == null || playerIds.Count == 0)
            {
                return default;
            }
            foreach (var playerId in playerIds)
            {
                var t = await _redisDb.StringGet<T>($"{queueName}_{channel}_{playerId}");
                list.Add(t);

                await _redisDb.SortedSetRemove($"{queueName}_{channel}", playerId);

                await _redisDb.KeyDelete($"{queueName}_{channel}_{playerId}");
            }
            return list;
        }
    }
}
