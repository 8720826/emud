using Emprise.Domain.Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Emprise.Infra.Data
{
    public class EmpriseDbContext : DbContext
    {
        private readonly IHostingEnvironment _env;

        public EmpriseDbContext(DbContextOptions<EmpriseDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            var typesToRegister = Assembly.Load("Emprise.Domain").GetTypes().Where(type => !string.IsNullOrEmpty(type.Namespace) && type.BaseType == typeof(BaseEntity));
            foreach (var entityType in typesToRegister)
            {
                if (modelBuilder.Model.FindEntityType(entityType) != null)
                {
                    continue;
                }
                modelBuilder.Model.AddEntityType(entityType);
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            /*
             * 
             * 
            // get the configuration from the app settings
            var config = new ConfigurationBuilder()
                .SetBasePath(_env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{_env.EnvironmentName}.json", optional: true)
                .Build();

            // define the database to use
            optionsBuilder.UseSqlServer(config.GetConnectionString("MsSql"));

            */

            base.OnConfiguring(optionsBuilder);
        }
    }
}
