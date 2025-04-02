using Landing.Core.Models;
using Landing.Core.Models.Events;
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
        public DbSet<News> News { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<EventAttendance> EventAttendances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Event>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<RegularEvent>("Regular")
                .HasValue<CuratedEvent>("Curated")
                .HasValue<OfflineEvent>("Offline");

            modelBuilder.Entity<CuratedEvent>()
                .HasMany(e => e.Curators)
                .WithMany()
                .UsingEntity(j => j.ToTable("EventCurators"));

            modelBuilder.ApplyConfiguration(new EventConfiguration());
            modelBuilder.ApplyConfiguration(new EventAttendanceConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        }
    }
}