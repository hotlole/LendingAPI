﻿using Landing.Core.Models;
using Landing.Core.Models.Events;
using Landing.Core.Models.Users;
using Landing.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Landing.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
            /*Database.EnsureDeleted();
            Database.EnsureCreated();*/

        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) 
        { 
            Console.WriteLine("Подключение к БД: " + options.FindExtension<RelationalOptionsExtension>()?.ConnectionString); 
        }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<News> News { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<EventAttendance> EventAttendances { get; set; }
        public DbSet<UserPointsTransaction> UserPointsTransactions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.ApplyConfiguration(new EventConfiguration());
            modelBuilder.ApplyConfiguration(new OfflineEventConfiguration());
            modelBuilder.ApplyConfiguration(new EventAttendanceConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new CuratedEventConfiguration());
            modelBuilder.ApplyConfiguration(new RegularEventConfiguration());
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var deletedEvents = ChangeTracker.Entries<Event>()
                .Where(e => e.State == EntityState.Deleted)
                .Select(e => e.Entity)
                .ToList();

            var deletedNews = ChangeTracker.Entries<News>()
                .Where(n => n.State == EntityState.Deleted)
                .Select(n => n.Entity)
                .ToList();

            var result = await base.SaveChangesAsync(cancellationToken);

            foreach (var ev in deletedEvents)
            {
                DeleteFileIfExists(ev.ImagePath);
            }

            foreach (var news in deletedNews)
            {
                DeleteFileIfExists(news.ImageUrl);
            }

            return result;
        }

        private void DeleteFileIfExists(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return;

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath.TrimStart('/'));
            if (File.Exists(path))
                File.Delete(path);
        }

    }
}