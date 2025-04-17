using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Landing.Infrastructure.Migrations
{
    public partial class AddPointsWithTrigger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Обновляем Points для всех текущих пользователей
            migrationBuilder.Sql(@"
                UPDATE ""Users"" u
                SET ""Points"" = (
                    SELECT COALESCE(SUM(""Points""), 0)  -- заменили Value на Points
                    FROM ""UserPointsTransactions"" upt
                    WHERE upt.""UserId"" = u.""Id""
                );
            ");

            // Функция обновления Points
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION update_user_points() RETURNS TRIGGER AS $$ 
                BEGIN
                    UPDATE ""Users""
                    SET ""Points"" = (
                        SELECT COALESCE(SUM(""Points""), 0)  -- заменили Value на Points
                        FROM ""UserPointsTransactions""
                        WHERE ""UserId"" = NEW.""UserId""
                    )
                    WHERE ""Id"" = NEW.""UserId"";
                    RETURN NULL;
                END;
                $$ LANGUAGE plpgsql;
            ");

            // Триггер на INSERT
            migrationBuilder.Sql(@"
                CREATE TRIGGER trg_update_points_after_insert
                AFTER INSERT ON ""UserPointsTransactions""
                FOR EACH ROW
                EXECUTE FUNCTION update_user_points();
            ");

            // Триггер на DELETE
            migrationBuilder.Sql(@"
                CREATE TRIGGER trg_update_points_after_delete
                AFTER DELETE ON ""UserPointsTransactions""
                FOR EACH ROW
                EXECUTE FUNCTION update_user_points();
            ");

            // Триггер на UPDATE
            migrationBuilder.Sql(@"
                CREATE TRIGGER trg_update_points_after_update
                AFTER UPDATE ON ""UserPointsTransactions""
                FOR EACH ROW
                EXECUTE FUNCTION update_user_points();
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Удаляем триггеры и функцию
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trg_update_points_after_insert ON ""UserPointsTransactions"";");
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trg_update_points_after_delete ON ""UserPointsTransactions"";");
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trg_update_points_after_update ON ""UserPointsTransactions"";");
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS update_user_points();");

            // Удаляем колонку
            migrationBuilder.DropColumn(
                name: "Points",
                table: "Users");
        }
    }
}
