using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Landing.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixFullNameMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Удаляем старую вычисляемую колонку
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Users");

            // Добавляем новую вычисляемую колонку с исправлением
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Users",
                type: "varchar(300)",
                maxLength: 300,
                nullable: true,
                computedColumnSql: "COALESCE(\"LastName\", '') || ' ' || \"FirstName\" || COALESCE(' ' || \"MiddleName\", '')",
                stored: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Удаляем вычисляемую колонку
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Users");

            // Возвращаем обратно старую версию
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Users",
                type: "varchar(300)",
                maxLength: 300,
                nullable: true,
                computedColumnSql: "\"LastName\" + ' ' + \"FirstName\" + CASE WHEN \"MiddleName\" IS NOT NULL AND \"MiddleName\" != '' THEN ' ' + \"MiddleName\" ELSE '' END",
                stored: false);
        }
    }
}
