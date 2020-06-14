using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Configuration;
using Emprise.Domain.Core.Models;
using Emprise.Infra.Authorization;
using Emprise.Infra.Bus;
using Emprise.Infra.Data;
using Emprise.Infra.Ioc;
using Emprise.Infra.IoC;
using Emprise.Infra.Middleware;
using Emprise.MudServer.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Savorboard.CAP.InMemoryMessageQueue;

namespace Emprise.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;

            var builder = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)

            .AddRedisConfiguration(configuration.GetValue<string>("Redis"), "configurations", 60);
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            #region ����ע��
            services.Configure<AppConfig>(Configuration);

            services.AddAuthentication("user").AddCookie("user", options =>
             {
                 options.LoginPath = new PathString("/user/login");
                 options.Events = new CookieAuthenticationEvents
                 {
                     OnValidatePrincipal = context => {
                         context.HttpContext.User = context.Principal;
                         return Task.CompletedTask;
                     }
                 };
             });
         
            services.AddAuthentication("admin").AddCookie("admin", options =>
            {
                options.AccessDeniedPath = "/Admin/Denied";
                options.LoginPath = "/Admin/Login";
            });

            services.AddRazorPages(options => {
                options.Conventions.AddPageRoute("/User/Login", "");
            }).AddMvcOptions(options =>
            {
                options.MaxModelValidationErrors = 50;
                options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(x => "����������");
                options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(x => $"����ֵ'{x}'��Ч");
                options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, y) => $"����ֵ'{x}'��Ч");
                options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor((x) => "ֻ����������");
                options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor((x) => $"ȱ������ '{x}'");
                options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() => "����������");
                options.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor((x) => "����ֵ��Ч");
                options.ModelBindingMessageProvider.SetMissingRequestBodyRequiredValueAccessor(() => "RequestBody ����Ϊ��");
                options.ModelBindingMessageProvider.SetNonPropertyValueMustBeANumberAccessor(() => "ֻ����������");
                options.ModelBindingMessageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor((x) => $"����ֵ'{x}'��Ч");
                options.ModelBindingMessageProvider.SetNonPropertyUnknownValueIsInvalidAccessor(() => "����ֵ��Ч");
            }); ;

            var dataProvide = Configuration.GetValue<string>("DataProvider").ToLower();

            #endregion

            services.AddDbContext<EmpriseDbContext>(x=> {
                switch (dataProvide)
                {
                    case "mssql":
                        x.UseSqlServer(Configuration.GetConnectionString(dataProvide));
                        break;
                    case "mysql":
                        x.UseMySql(Configuration.GetConnectionString(dataProvide));
                        break;
                    case "postgresql":
                        x.UseNpgsql(Configuration.GetConnectionString(dataProvide));
                        break;
                    default:
                        throw new Exception("���ݿ��������ô�������appsettings.json�ļ���");
                }
            });

            services.AddCap(x =>
            {
                switch (dataProvide)
                {
                    case "mssql":
                        x.UseSqlServer(Configuration.GetConnectionString(dataProvide));
                        break;
                    case "mysql":
                        x.UseMySql(Configuration.GetConnectionString(dataProvide));
                        break;
                    case "postgresql":
                        x.UsePostgreSql(Configuration.GetConnectionString(dataProvide));
                        break;
                    default:
                        throw new Exception("���ݿ��������ô�������appsettings.json�ļ���");
                }

                x.Version = "v1";
                x.SucceedMessageExpiredAfter = 1 * 3600;
                x.ConsumerThreadCount = 5;
                x.FailedRetryCount = 3;
                x.FailedRetryInterval = 30;
                x.UseInMemoryMessageQueue();
            });

            //�Զ�ע��
            services.AutoRegister();

            //�ֶ�ע�룬�޷��Զ�ע���
            NativeInjectorBootStrapper.RegisterServices(services);
          
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            RequestLocalizationOptions localizationOptions = new RequestLocalizationOptions
            {
                SupportedCultures = new List<CultureInfo> { new CultureInfo("zh-CN"), new CultureInfo("zh-CN") },
                SupportedUICultures = new List<CultureInfo> { new CultureInfo("zh-CN"), new CultureInfo("zh-CN") },
                DefaultRequestCulture = new RequestCulture("zh-CN")
            };
            app.UseRequestLocalization(localizationOptions);

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<MudHub>("/hub");

                endpoints.MapRazorPages();
            });

        }

    }
}
