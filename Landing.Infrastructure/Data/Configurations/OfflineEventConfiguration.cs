using Landing.Core.Models.Events;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class OfflineEventConfiguration : IEntityTypeConfiguration<OfflineEvent>
{
    public void Configure(EntityTypeBuilder<OfflineEvent> builder)
    {
        builder
            .Property(e => e.Latitude)
            .HasPrecision(9, 6);

        builder
            .Property(e => e.Longitude)
            .HasPrecision(9, 6);
    }
}
