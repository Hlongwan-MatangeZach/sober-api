using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoberPath_API.Migrations
{
    /// <inheritdoc />
    public partial class sgdgfjvgcgxvtsv7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DailyThreshold",
                table: "Substances",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "unit",
                table: "Substances",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyThreshold",
                table: "Substances");

            migrationBuilder.DropColumn(
                name: "unit",
                table: "Substances");
        }
    }
}
