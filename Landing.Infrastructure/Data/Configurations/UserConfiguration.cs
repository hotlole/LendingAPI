using Landing.Core.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Landing.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
                .Property(u => u.BirthDate)
                .HasConversion(
                    v => v.HasValue ? v.Value.ToUniversalTime() : (DateTime?)null,
                    v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null

                );
            // Используем функцию для вычисления суммы баллов
            builder
                .Property(u => u.Points)
                .HasComputedColumnSql(
                    @"
                    calculate_user_points(""Id"")",
                    stored: true
                );

            builder
               .Property(u => u.FullName)
               .HasComputedColumnSql(
                   @"
                    ""LastName"" || ' ' || ""FirstName"" || 
                    CASE 
                        WHEN ""MiddleName"" IS NOT NULL AND ""MiddleName"" != ''
                        THEN ' ' || ""MiddleName""
                        ELSE ''
                    END",
                   stored: true
               );
        }
    }
}
