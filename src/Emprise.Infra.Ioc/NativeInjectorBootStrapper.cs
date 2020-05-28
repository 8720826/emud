using AutoMapper;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Notifications;
using Emprise.Infra.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Emprise.Infra.Mapper;
using Microsoft.Extensions.Configuration;

namespace Emprise.Infra.IoC
{
    public class NativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services,string dataProvide, IConfiguration configuration)
        {


            var assembly = AppDomain.CurrentDomain.Load("Emprise.MudServer");
            services.AddMediatR(assembly);

            services.AddHttpClient();

            services.AddSignalR();


            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSignalR();

            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

            services.AddAutoMapper();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();

        }

    }
}
