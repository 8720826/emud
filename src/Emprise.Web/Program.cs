using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Emprise.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
            .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                .ReadFrom.Configuration(hostingContext.Configuration)
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                //.WriteTo.File(Path.Combine("logs", @"log.txt"), rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Information)
                .WriteTo.MySQL(hostingContext.Configuration.GetSection("ConnectionStrings:MySql").Value,"SystemLog", LogEventLevel.Debug)
               
               
                .WriteTo.Console()
            );
    }
}
