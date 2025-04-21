using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Landing.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFullNameComputedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Добавляем столбец FullName с вычисляемым значением
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='Users' AND column_name='FullName') THEN
                        ALTER TABLE ""Users"" ADD ""FullName"" character varying(300) GENERATED ALWAYS AS (
                            ""LastName"" || ' ' || ""FirstName"" || 
                            CASE 
                                WHEN ""MiddleName"" IS NOT NULL AND ""MiddleName"" != ''
                                THEN ' ' || ""MiddleName""
                                ELSE ''
                            END
                        ) STORED;
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Удаляем столбец FullName
            migrationBuilder.Sql("ALTER TABLE \"Users\" DROP COLUMN IF EXISTS \"FullName\";");
        }
    }
}
