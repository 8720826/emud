using Emprise.Domain.Core.Interfaces.Ioc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emprise.Domain.Core.Interfaces
{
    public interface IRedisDb: IScoped
    {
        Task<bool> StringSet<T>(string key, T t, DateTime? expiry = null);

        Task<T> StringGet<T>(string key);

        Task KeyExpire(string key, DateTime expiry);

        Task<long> KeyTimeToLive(string key);

        Task<bool> KeyDelete(string key);

        Task<Dictionary<string, T>> HashGetAll<T>(string key);

        Task<T> HashGet<T>(string key, string field);

        Task<bool> HashDelete(string key, string field);

        Task<bool> HashSet<T>(string key, string field, T t);

        Task<bool> SortedSetAdd(string key, string value, double score);

        Task<bool> SortedSetRemove(string key, string value);

        Task<List<string>> SortedSetRangeByScore(string key, double score);

        Task<long> Publish(string channel, string message);

        Task Subscribe(string channel, Action<string, string> action);
    }
}
