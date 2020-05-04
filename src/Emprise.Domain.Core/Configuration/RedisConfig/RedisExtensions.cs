using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Core.Configuration
{
    public static class RedisExtensions
    {
        public static IConfigurationBuilder AddRedisConfiguration(
            this IConfigurationBuilder builder, string redisString, string configKey, int reloadTime)
        {
            return builder.Add(new RedisConfigurationSource(redisString, configKey, reloadTime));
        }
    }
}
