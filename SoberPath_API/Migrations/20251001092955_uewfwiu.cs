using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoberPath_API.Migrations
{
    /// <inheritdoc />
    public partial class uewfwiu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "Users",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "Users");
        }
    }
}
