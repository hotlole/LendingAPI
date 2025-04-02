using Landing.Core.Models.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Landing.Infrastructure.Data.Configurations
{
    public class EventConfiguration : IEntityTypeConfiguration<OfflineEvent>
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
}
