using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoberPath_API.Migrations
{
    /// <inheritdoc />
    public partial class sfdghfjgkl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_rooms_RoomId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_RoomId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "CurrentOccupancy",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "rooms");

            migrationBuilder.AlterColumn<string>(
                name: "RoomNumber",
                table: "rooms",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "BuildingName",
                table: "rooms",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "AllocatedDate",
                table: "rooms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "rooms",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "expectedCheckOutDate",
                table: "rooms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_rooms_ClientId",
                table: "rooms",
                column: "ClientId",
                unique: true,
                filter: "[ClientId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_rooms_Users_ClientId",
                table: "rooms",
                column: "ClientId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rooms_Users_ClientId",
                table: "rooms");

            migrationBuilder.DropIndex(
                name: "IX_rooms_ClientId",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "AllocatedDate",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "expectedCheckOutDate",
                table: "rooms");

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RoomNumber",
                table: "rooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BuildingName",
                table: "rooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentOccupancy",
                table: "rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoomId",
                table: "Users",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_rooms_RoomId",
                table: "Users",
                column: "RoomId",
                principalTable: "rooms",
                principalColumn: "Id");
        }
    }
}
