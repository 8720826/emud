using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emprise.Admin.Mapper
{
    public static class AutoMapperExtension
    {
        public static void AddAutoMapper(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            //添加服务
            //可以添加筛选
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies().Where(t => t.GetName().ToString().StartsWith("Emprise.")));
            //启动配置

            AutoMapperConfig.RegisterMappings();
        }


    }
}
