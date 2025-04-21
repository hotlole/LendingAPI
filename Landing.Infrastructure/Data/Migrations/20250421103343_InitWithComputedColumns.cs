using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Landing.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitWithComputedColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Создаем функцию для подсчета баллов, если она еще не существует
            migrationBuilder.Sql(@"
        CREATE OR REPLACE FUNCTION calculate_user_points(user_id uuid)
        RETURNS integer AS $$
        BEGIN
            RETURN (
                SELECT COALESCE(SUM(""Points""), 0)
                FROM ""UserPointsTransactions""
                WHERE ""UserId"" = user_id
            );
        END;
        $$ LANGUAGE plpgsql;
    ");

            // Проверяем, существует ли столбец "Points", и если нет, добавляем его
            migrationBuilder.Sql(@"
        DO $$ 
        BEGIN 
            IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='Users' AND column_name='Points') THEN
                ALTER TABLE ""Users"" ADD ""Points"" integer GENERATED ALWAYS AS (calculate_user_points(""Id"")) STORED NOT NULL;
            END IF; 
        END $$;
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Удаляем функцию при откате миграции
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS calculate_user_points;");

            // Удаляем столбцы при откате
            migrationBuilder.DropColumn(
                name: "Points",
                table: "Users"
            );

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Users"
            );
        }
    }
}
