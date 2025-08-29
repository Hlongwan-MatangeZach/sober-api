using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoberPath_API.Migrations
{
    /// <inheritdoc />
    public partial class SB2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Users_Social_WorkerId",
                table: "Event");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Event",
                table: "Event");

            migrationBuilder.RenameTable(
                name: "Event",
                newName: "Events");

            migrationBuilder.RenameIndex(
                name: "IX_Event_Social_WorkerId",
                table: "Events",
                newName: "IX_Events_Social_WorkerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Events",
                table: "Events",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Users_Social_WorkerId",
                table: "Events",
                column: "Social_WorkerId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Users_Social_WorkerId",
                table: "Events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Events",
                table: "Events");

            migrationBuilder.RenameTable(
                name: "Events",
                newName: "Event");

            migrationBuilder.RenameIndex(
                name: "IX_Events_Social_WorkerId",
                table: "Event",
                newName: "IX_Event_Social_WorkerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Event",
                table: "Event",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Users_Social_WorkerId",
                table: "Event",
                column: "Social_WorkerId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
