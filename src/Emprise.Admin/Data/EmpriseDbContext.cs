
using Emprise.Domain.Admin.Entity;
using Emprise.Domain.Npc.Entity;
using Emprise.Domain.Room.Entity;
using Emprise.Domain.Tasks.Entity;
using Emprise.Domain.Ware.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Emprise.Admin.Data
{
    public class EmpriseDbContext : DbContext
    {

        public EmpriseDbContext(DbContextOptions<EmpriseDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<RoomEntity> Rooms { get; set; }

        public DbSet<AdminEntity> Admins { get; set; }

        public DbSet<NpcEntity> Npcs { get; set; }

        public DbSet<WareEntity> Wares { get; set; }

        public DbSet<TaskEntity> Tasks { get; set; }


        public DbSet<NpcScriptEntity> NpcScripts { get; set; }


        public DbSet<NpcScriptCommandEntity> NpcScriptCommands { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            base.OnConfiguring(optionsBuilder);
        }
    }
}
