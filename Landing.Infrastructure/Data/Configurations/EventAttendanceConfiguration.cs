using Landing.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Landing.Infrastructure.Data.Configurations
{
    public class EventAttendanceConfiguration : IEntityTypeConfiguration<EventAttendance>
    {
        public void Configure(EntityTypeBuilder<EventAttendance> builder)
        {
            builder
                .HasOne(ea => ea.User)
                .WithMany(u => u.Attendances)
                .HasForeignKey(ea => ea.UserId);

            builder
                .HasOne(ea => ea.Event)
                .WithMany(e => e.Attendances)
                .HasForeignKey(ea => ea.EventId);
        }
    }
}
