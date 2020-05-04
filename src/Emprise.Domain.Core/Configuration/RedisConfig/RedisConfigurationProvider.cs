using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Core.Configuration
{
    public class RedisConfigurationProvider : ConfigurationProvider
    {
        private RedisConfigurationSource _source;
        private readonly Task _configurationListeningTask;

        public RedisConfigurationProvider(RedisConfigurationSource source)
        {
            _source = source;
            _configurationListeningTask = new Task(ListenToConfigurationChanges);
        }

        public override void Load()
        {
            LoadAsync().GetAwaiter();
        }

        public async Task LoadAsync()
        {
            await LoadData();

            //启动配置更改监听
            if (_configurationListeningTask.Status == TaskStatus.Created)
                _configurationListeningTask.Start();
        }

        private async void ListenToConfigurationChanges()
        {
            if (_source.ReloadTime <= 0)
                return;

            while (true)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(_source.ReloadTime));
                    await LoadData();
                    
                    OnReload();
                }
                catch
                {
                }
            }
        }

        public async Task LoadData()
        {
            var redis = ConnectionMultiplexer.Connect(_source.RedisString);
            var configurations = redis.GetDatabase().HashGetAll(_source.ConfigKey);

            if (configurations.Length == 0)
                return;

            var dictionary = configurations.ToDictionary(it => it.Name.ToString(), it => it.Value.ToString());
            Data = new Dictionary<string, string>(dictionary, StringComparer.OrdinalIgnoreCase);


        }


    }
}
