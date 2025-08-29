using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoberPath_API.Migrations
{
    /// <inheritdoc />
    public partial class SB23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Assignment_Date",
                table: "SessionBookings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<int>(
                name: "Stime",
                table: "SessionBookings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Stype",
                table: "SessionBookings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stime",
                table: "SessionBookings");

            migrationBuilder.DropColumn(
                name: "Stype",
                table: "SessionBookings");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Assignment_Date",
                table: "SessionBookings",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
