
using Emprise.Domain.Admin.Entity;
using Emprise.Domain.Npc.Entity;
using Emprise.Domain.Quest.Entity;
using Emprise.Domain.Room.Entity;
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

        public DbSet<QuestEntity> Quests { get; set; }


        public DbSet<ScriptEntity> Scripts { get; set; }

        public DbSet<NpcScriptEntity> NpcScripts { get; set; }
        


        public DbSet<ScriptCommandEntity> ScriptCommands { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NpcScriptEntity>()
          .HasKey(t => new { t.NpcId, t.ScriptId });


            modelBuilder.Entity<NpcScriptEntity>()
                .HasOne(pt => pt.Npc)
                .WithMany(p => p.NpcScripts)
                .HasForeignKey(pt => pt.NpcId);

            modelBuilder.Entity<NpcScriptEntity>()
                .HasOne(pt => pt.Script)
                .WithMany(t => t.NpcScripts)
                .HasForeignKey(pt => pt.ScriptId);



            modelBuilder.Entity<ScriptCommandEntity>()
           .HasOne(p => p.Script)
           .WithMany(b => b.ScriptCommands);



            base.OnModelCreating(modelBuilder);
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            base.OnConfiguring(optionsBuilder);
        }
    }
}
