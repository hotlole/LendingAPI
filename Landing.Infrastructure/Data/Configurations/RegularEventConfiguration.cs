using Landing.Core.Models.Events;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;

namespace Landing.Infrastructure.Data.Configurations
{
    public class RegularEventConfiguration : IEntityTypeConfiguration<RegularEvent>
    {
        public void Configure(EntityTypeBuilder<RegularEvent> builder)
        {
            builder
                .HasMany(e => e.Participants)
                .WithMany()
                .UsingEntity(j => j.ToTable("EventParticipants"));
        }
    }
    
}

