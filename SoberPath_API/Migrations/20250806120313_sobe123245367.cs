using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoberPath_API.Migrations
{
    /// <inheritdoc />
    public partial class sobe123245367 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsecutiveUseMultiplier",
                table: "Substances");

            migrationBuilder.DropColumn(
                name: "ExcessScoreFactor",
                table: "Substances");

            migrationBuilder.DropColumn(
                name: "ExcessScoreUnit",
                table: "Substances");

            migrationBuilder.DropColumn(
                name: "FrequencyPenaltyFactor",
                table: "Substances");

            migrationBuilder.DropColumn(
                name: "HistoricalDecayFactor",
                table: "Substances");

            migrationBuilder.DropColumn(
                name: "NormalizationCap",
                table: "Substances");

            migrationBuilder.DropColumn(
                name: "SafeLimitPerDay",
                table: "Substances");

            migrationBuilder.DropColumn(
                name: "StandardUnit",
                table: "Substances");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ConsecutiveUseMultiplier",
                table: "Substances",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ExcessScoreFactor",
                table: "Substances",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ExcessScoreUnit",
                table: "Substances",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "FrequencyPenaltyFactor",
                table: "Substances",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "HistoricalDecayFactor",
                table: "Substances",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "NormalizationCap",
                table: "Substances",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SafeLimitPerDay",
                table: "Substances",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "StandardUnit",
                table: "Substances",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
