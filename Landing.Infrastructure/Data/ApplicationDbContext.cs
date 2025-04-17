using Landing.Core.Models;
using Landing.Core.Models.Events;
using Landing.Core.Models.News;
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
        public DbSet<NewsImage> NewsImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<NewsImage>()
                .HasOne(i => i.News)
                .WithMany(n => n.AdditionalImages)
                .HasForeignKey(i => i.NewsId)
                .OnDelete(DeleteBehavior.Cascade);
                
            modelBuilder.ApplyConfiguration(new EventConfiguration());
            modelBuilder.ApplyConfiguration(new OfflineEventConfiguration());
            modelBuilder.ApplyConfiguration(new EventAttendanceConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new CuratedEventConfiguration());
            modelBuilder.ApplyConfiguration(new RegularEventConfiguration());
        }
    }
}