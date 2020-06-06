using System;
using System.Collections.Generic;
using System.Linq;
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
            #region 基本注入
            services.Configure<AppConfig>(Configuration);


            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).
             AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
             {
                 o.LoginPath = new PathString("/user/login");
             });

            if (Configuration.GetValue<bool>("Site:IsApiEnable"))
            {
                services.AddAuthentication().AddJwtBearerAuth(Configuration.GetValue<string>("Site:ApiKey"));
            }
            else
            {
                services.AddAuthentication().AddJwtBearerAuth();
            }


            services.AddControllers().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var error = actionContext.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .Select(e => new
                        {
                            status = false,
                            errorMessage = e.Value.Errors.First().ErrorMessage
                        }).First();


                    return new OkObjectResult(error);
                };
            });

            services.AddRazorPages(options => {
                options.Conventions.AddPageRoute("/User/Login", "");
            });



            var dataProvide = Configuration.GetValue<string>("DataProvider").ToLower();

            #endregion

            services.AddScoped<IQueueHandler, QueueCapBus>();

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
                        throw new Exception("数据库链接配置错误，请检查appsettings.json文件！");
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
                        throw new Exception("数据库链接配置错误，请检查appsettings.json文件！");
                }

                x.Version = "v1";
                x.SucceedMessageExpiredAfter = 1 * 3600;
                x.ConsumerThreadCount = 5;
                x.FailedRetryCount = 3;
                x.FailedRetryInterval = 30;
                x.UseInMemoryMessageQueue();
            });

            //自动注入
            services.AutoRegister();


            //手动注入，无法自动注入的
            NativeInjectorBootStrapper.RegisterServices(services);

           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseStaticFiles();

            /*
            app.UseHangfireServer(
               new BackgroundJobServerOptions
               {
                   SchedulePollingInterval = TimeSpan.FromSeconds(1),
               });
            
            app.UseHangfireDashboard();
            */
            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<MudHub>("/hub");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "areas", "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });

        }

    }
}
