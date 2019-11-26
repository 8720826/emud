using Emprise.Domain.Core.Interfaces.Ioc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;



namespace Emprise.Infra.Ioc
{
    public static  class ServiceCollectionExtension
    {
        public static void AutoRegister(this IServiceCollection services)
        {
            #region 自动注入

            var allAssemblies = Assembly.GetEntryAssembly().GetReferencedAssemblies().Select(Assembly.Load);
            foreach (var serviceAsm in allAssemblies)
            {
                var serviceList = serviceAsm.GetTypes().Where(t => t.IsClass && !t.IsAbstract && !t.IsInterface);

                foreach (Type serviceType in serviceList.Where(t => typeof(IScoped).IsAssignableFrom(t)))
                {
                    var interfaceTypes = serviceType.GetInterfaces();

                    foreach (var interfaceType in interfaceTypes)
                    {
                        services.AddScoped(interfaceType, serviceType);
                    }
                }

                foreach (Type serviceType in serviceList.Where(t => typeof(ISingleton).IsAssignableFrom(t)))
                {
                    var interfaceTypes = serviceType.GetInterfaces();

                    foreach (var interfaceType in interfaceTypes)
                    {
                        services.AddSingleton(interfaceType, serviceType);
                    }
                }

                foreach (Type serviceType in serviceList.Where(t => typeof(ITransient).IsAssignableFrom(t)))
                {
                    var interfaceTypes = serviceType.GetInterfaces();

                    foreach (var interfaceType in interfaceTypes)
                    {
                        services.AddTransient(interfaceType, serviceType);
                    }
                }

                foreach (Type serviceType in serviceList.Where(t => t.IsSubclassOf(typeof(BackgroundService))))
                {
                    services.AddTransient(typeof(IHostedService), serviceType);
                }
            }
            #endregion

        }

    }
}
