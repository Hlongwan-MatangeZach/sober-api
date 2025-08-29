using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoberPath_API.Migrations
{
    /// <inheritdoc />
    public partial class soberpath12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FilePaths",
                table: "Applications",
                newName: "FileName");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Data",
                table: "Applications",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Data",
                table: "Applications");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "Applications",
                newName: "FilePaths");
        }
    }
}
