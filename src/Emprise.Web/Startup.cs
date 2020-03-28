using System;
using System.Collections.Generic;
using System.Linq;
using Emprise.Domain.Core.Models;
using Emprise.Infra.Authorization;
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
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

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
               .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
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


           

            //services.AddDbContext<EmpriseDbContext>(options => options.UseSqlServer(Configuration.GetValue<string>("ConnectionStrings:MsSql"), b => b.UseRowNumberForPaging()));

            //services.AddDbContext<EmpriseDbContext>(options => options.UseMySql(Configuration.GetValue<string>("ConnectionStrings:Mysql")));
            services.AddDbContext<EmpriseDbContext>( // replace "YourDbContext" with the class name of your DbContext
               options => options.UseMySql(Configuration.GetValue<string>("ConnectionStrings:Mysql"), // replace with your Connection String
                   mySqlOptions =>
                   {
                       mySqlOptions.ServerVersion(new Version(5, 7, 17), ServerType.MySql); // replace with your Server Version and Type
                    }
           ));


            #endregion


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
