using Landing.Core.Models.Events;
using Landing.Core.Models.News;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;


namespace Landing.Infrastructure.Data.Configurations
{
    internal class NewsConfiguration : IEntityTypeConfiguration<News>
    {
        public void Configure(EntityTypeBuilder<News> builder)
        {
            builder
                .HasIndex(n => n.VkPostId)
                .IsUnique()
                .HasFilter("VkPostId IS NOT NULL");

        }
    }
}
