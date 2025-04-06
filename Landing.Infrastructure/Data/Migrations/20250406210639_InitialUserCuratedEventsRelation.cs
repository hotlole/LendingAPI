using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Landing.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialUserCuratedEventsRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Events_RegularEventId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "EventCurators");

            migrationBuilder.DropIndex(
                name: "IX_Users_RegularEventId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RegularEventId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Events");

            migrationBuilder.CreateTable(
                name: "EventParticipants",
                columns: table => new
                {
                    ParticipantsId = table.Column<int>(type: "integer", nullable: false),
                    RegularEventId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventParticipants", x => new { x.ParticipantsId, x.RegularEventId });
                    table.ForeignKey(
                        name: "FK_EventParticipants_Events_RegularEventId",
                        column: x => x.RegularEventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventParticipants_Users_ParticipantsId",
                        column: x => x.ParticipantsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCuratedEvents",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    EventId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCuratedEvents", x => new { x.UserId, x.EventId });
                    table.ForeignKey(
                        name: "FK_UserCuratedEvents_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCuratedEvents_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipants_RegularEventId",
                table: "EventParticipants",
                column: "RegularEventId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCuratedEvents_EventId",
                table: "UserCuratedEvents",
                column: "EventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventParticipants");

            migrationBuilder.DropTable(
                name: "UserCuratedEvents");

            migrationBuilder.AddColumn<int>(
                name: "RegularEventId",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Events",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "EventCurators",
                columns: table => new
                {
                    CuratedEventId = table.Column<int>(type: "integer", nullable: false),
                    CuratorsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventCurators", x => new { x.CuratedEventId, x.CuratorsId });
                    table.ForeignKey(
                        name: "FK_EventCurators_Events_CuratedEventId",
                        column: x => x.CuratedEventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventCurators_Users_CuratorsId",
                        column: x => x.CuratorsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_RegularEventId",
                table: "Users",
                column: "RegularEventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventCurators_CuratorsId",
                table: "EventCurators",
                column: "CuratorsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Events_RegularEventId",
                table: "Users",
                column: "RegularEventId",
                principalTable: "Events",
                principalColumn: "Id");
        }
    }
}
