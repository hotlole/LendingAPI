using Landing.Core.Models.Events;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Landing.Core.Models;

public class CuratedEventConfiguration : IEntityTypeConfiguration<CuratedEvent>
{
    public void Configure(EntityTypeBuilder<CuratedEvent> builder)
    {
        builder
            .HasMany(e => e.Curators) // CuratedEvent.Curators
            .WithMany(u => u.CuratedEvents) // User.CuratedEvents
            .UsingEntity<Dictionary<string, object>>(
                "UserCuratedEvents",

                j => j
                    .HasOne<User>()
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<CuratedEvent>()
                    .WithMany()
                    .HasForeignKey("EventId") 
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("UserId", "EventId"); 
                    j.ToTable("UserCuratedEvents");
               

                });

    }
}