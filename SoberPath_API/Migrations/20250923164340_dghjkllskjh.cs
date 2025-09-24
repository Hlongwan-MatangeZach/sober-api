using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoberPath_API.Migrations
{
    /// <inheritdoc />
    public partial class dghjkllskjh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "RoomAllocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    AllocationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpectedCheckOutDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualCheckOutDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomAllocations_Users_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomAllocations_rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoomId",
                table: "Users",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomAllocations_ClientId",
                table: "RoomAllocations",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomAllocations_RoomId",
                table: "RoomAllocations",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_rooms_RoomId",
                table: "Users",
                column: "RoomId",
                principalTable: "rooms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_rooms_RoomId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "RoomAllocations");

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
    }
}
