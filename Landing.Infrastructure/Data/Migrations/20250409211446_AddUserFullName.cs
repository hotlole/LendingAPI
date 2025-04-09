using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Landing.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserFullName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
    name: "FullName",
    table: "Users",
    type: "nvarchar(300)",
    maxLength: 300,
    nullable: true,
    computedColumnSql: "[LastName] + ' ' + [FirstName] + CASE WHEN [MiddleName] IS NOT NULL AND [MiddleName] != '' THEN ' ' + [MiddleName] ELSE '' END",
    stored: false);


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Users");
        }
    }
}
