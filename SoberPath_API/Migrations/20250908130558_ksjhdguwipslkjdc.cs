using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoberPath_API.Migrations
{
    /// <inheritdoc />
    public partial class ksjhdguwipslkjdc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Rehab_disharges_ApplicationId",
                table: "Rehab_disharges",
                column: "ApplicationId",
                unique: true,
                filter: "[ApplicationId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Rehab_disharges_Applications_ApplicationId",
                table: "Rehab_disharges",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rehab_disharges_Applications_ApplicationId",
                table: "Rehab_disharges");

            migrationBuilder.DropIndex(
                name: "IX_Rehab_disharges_ApplicationId",
                table: "Rehab_disharges");
        }
    }
}
