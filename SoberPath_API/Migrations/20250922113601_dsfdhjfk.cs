using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoberPath_API.Migrations
{
    /// <inheritdoc />
    public partial class dsfdhjfk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpectedCheckOut",
                table: "rooms",
                newName: "expectedCheckOutDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "expectedCheckOutDate",
                table: "rooms",
                newName: "ExpectedCheckOut");
        }
    }
}
