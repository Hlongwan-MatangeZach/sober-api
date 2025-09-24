using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoberPath_API.Migrations
{
    /// <inheritdoc />
    public partial class dfghjk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExpectedCheckOut",
                table: "rooms",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpectedCheckOut",
                table: "rooms");
        }
    }
}
