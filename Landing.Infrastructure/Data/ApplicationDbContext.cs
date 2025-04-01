﻿using Landing.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Landing.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();

        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) 
        { 
            Console.WriteLine("Подключение к БД: " + options.FindExtension<RelationalOptionsExtension>()?.ConnectionString); 
        }

        public DbSet<User> Users { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<EventAttendance> EventAttendances { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);
            modelBuilder.Entity<EventAttendance>().HasOne(ea => ea.User)
                .WithMany(u => u.Attendances)
                .HasForeignKey(ea => ea.UserId);

            modelBuilder.Entity<EventAttendance>()
                .HasOne(ea => ea.Event)
                .WithMany(e => e.Attendances)
                .HasForeignKey(ea => ea.EventId);
            modelBuilder.Entity<Event>()
                .Property(e => e.Latitude)
                .HasPrecision(9, 6);
            modelBuilder.Entity<Event>()
                .Property(e => e.Longitude)
                .HasPrecision(9, 6);


        }
    }
}