using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Landing.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeFullNameStored : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Удаляем старую вычисляемую колонку
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Users");

            // Добавляем новую с stored: true
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Users",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                computedColumnSql: "[LastName] + ' ' + [FirstName] + CASE WHEN [MiddleName] IS NOT NULL AND [MiddleName] != '' THEN ' ' + [MiddleName] ELSE '' END",
                stored: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Удаляем stored-колонку
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Users");

            // Возвращаем обратно несторированную версию
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Users",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                computedColumnSql: "[LastName] + ' ' + [FirstName] + CASE WHEN [MiddleName] IS NOT NULL AND [MiddleName] != '' THEN ' ' + [MiddleName] ELSE '' END",
                stored: false);
        }
    }
}
