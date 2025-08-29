using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoberPath_API.Migrations
{
    /// <inheritdoc />
    public partial class SoberPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "hasRelapse",
                table: "Applications",
                newName: "HasRelapse");

            migrationBuilder.RenameColumn(
                name: "Rehab_Reson",
                table: "Applications",
                newName: "Update_Comment");

            migrationBuilder.AddColumn<string>(
                name: "RehabReason",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status_Update_Date",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RehabReason",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Status_Update_Date",
                table: "Applications");

            migrationBuilder.RenameColumn(
                name: "HasRelapse",
                table: "Applications",
                newName: "hasRelapse");

            migrationBuilder.RenameColumn(
                name: "Update_Comment",
                table: "Applications",
                newName: "Rehab_Reson");
        }
    }
}
