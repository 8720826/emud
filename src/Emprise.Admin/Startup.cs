using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Admin.Api;
using Emprise.Admin.Data;
using Emprise.Admin.Helper;
using Emprise.Admin.Mapper;
using Emprise.Domain.Core.Configuration;
using Emprise.Domain.Core.Models;
using Emprise.Infra.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Emprise.Admin
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;

            var builder = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)

            .AddRedisConfiguration(configuration.GetConnectionString("Redis"), "configurations", 60);

            Configuration = builder.Build();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppConfig>(Configuration);


            services.AddRazorPages().AddRazorPagesOptions(options =>
            {
                options.Conventions.AuthorizePage("/Index");
                options.Conventions.AuthorizeFolder("/Room");
                options.Conventions.AuthorizeFolder("/Config");
                options.Conventions.AuthorizeFolder("/Npc");
                options.Conventions.AuthorizeFolder("/Script");
                options.Conventions.AuthorizeFolder("/Ware");
                options.Conventions.AuthorizeFolder("/Quest");
                options.Conventions.AuthorizeFolder("/User");
                options.Conventions.AuthorizeFolder("/Player");
                options.Conventions.AuthorizeFolder("/Log");


                options.Conventions.AllowAnonymousToPage("/Login");
                options.Conventions.AllowAnonymousToPage("/Denied");


            }).AddMvcOptions(options =>
            {
                options.MaxModelValidationErrors = 50;
                options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(x => "请输入内容");
                options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(x => $"输入值'{x}'无效");
                options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x,y) => $"输入值'{x}'无效");
                options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor((x) => "只能输入数字");
                options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor((x) => $"缺少属性 '{x}'");
                options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() => "请输入内容");
                options.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor((x) => "输入值无效");
                options.ModelBindingMessageProvider.SetMissingRequestBodyRequiredValueAccessor(() => "RequestBody 不能为空");
                options.ModelBindingMessageProvider.SetNonPropertyValueMustBeANumberAccessor(() => "只能输入数字");
                options.ModelBindingMessageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor((x) => $"输入值'{x}'无效");
                options.ModelBindingMessageProvider.SetNonPropertyUnknownValueIsInvalidAccessor(() => "输入值无效");
            });

            services.AddAuthentication("admin").AddCookie("admin", options =>
            {
                options.AccessDeniedPath = "/Denied";
                options.LoginPath = "/Login";
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IOssHelper, OssHelper>();

            services.AddTransient<AuthorizationHandler>();

            services.AddMudRefit(Configuration.GetValue<string>("Site:Url"))
              .AddHttpMessageHandler<AuthorizationHandler>()
             ;

            services.AddDbContext<EmpriseDbContext>( // replace "YourDbContext" with the class name of your DbContext
               options => options.UseMySql(Configuration.GetConnectionString("Mysql"), // replace with your Connection String
                   mySqlOptions =>
                   {
                       mySqlOptions.ServerVersion(new Version("5.7.17"), ServerType.MySql); // replace with your Server Version and Type
                               }
            ));

            services.AddAutoMapper();

  
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            RequestLocalizationOptions localizationOptions = new RequestLocalizationOptions
            {
                SupportedCultures = new List<CultureInfo> { new CultureInfo("zh-CN"), new CultureInfo("zh-CN") },
                SupportedUICultures = new List<CultureInfo> { new CultureInfo("zh-CN"), new CultureInfo("zh-CN") },
                DefaultRequestCulture = new RequestCulture("zh-CN")
            };
            app.UseRequestLocalization(localizationOptions);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<ErrorHandlingMiddleware>();



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
