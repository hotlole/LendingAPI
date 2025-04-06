using Landing.Core.Models.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Landing.Infrastructure.Data.Configurations
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder
               .HasDiscriminator<EventType>("Type")
               .HasValue<Event>(EventType.Base)
               .HasValue<RegularEvent>(EventType.Regular)
               .HasValue<CuratedEvent>(EventType.Curated)
               .HasValue<OfflineEvent>(EventType.Offline);
        }
    }
}
