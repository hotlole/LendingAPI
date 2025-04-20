using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Landing.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddComputedPointsToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
  ALTER TABLE ""Users""
  ADD COLUMN ""Points"" integer
  GENERATED ALWAYS AS (
    COALESCE((
      SELECT SUM(""Points"")
      FROM ""UserPointsTransactions""
      WHERE ""UserId"" = ""Users"".""Id""
    ), 0) +
    COALESCE((
      SELECT SUM(""PointsAwarded"")
      FROM ""EventAttendances""
      WHERE ""UserId"" = ""Users"".""Id"" AND ""IsConfirmed"" = true
    ), 0)
  ) STORED;
");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
