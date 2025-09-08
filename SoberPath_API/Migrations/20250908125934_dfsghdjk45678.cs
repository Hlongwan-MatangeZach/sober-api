using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoberPath_API.Migrations
{
    /// <inheritdoc />
    public partial class dfsghdjk45678 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Recieved_Applications");

            migrationBuilder.DropTable(
                name: "SessionBookings");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Addiction_level",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Discharge_Date",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Discharge_Reason",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "RehabReason",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Substance_type",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Update_Comment",
                table: "Applications");

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "Next_Of_Kins",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Client_Id",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NGO_AdminId",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NGO_Id",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rehab_AdminID",
                table: "Applications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Rehab_disharges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: true),
                    Disharge_Date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Disharge_Reason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rehab_disharges", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_ClientId",
                table: "Events",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_NGO_AdminId",
                table: "Events",
                column: "NGO_AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_Rehab_AdminID",
                table: "Applications",
                column: "Rehab_AdminID");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Users_Rehab_AdminID",
                table: "Applications",
                column: "Rehab_AdminID",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Users_ClientId",
                table: "Events",
                column: "ClientId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Users_NGO_AdminId",
                table: "Events",
                column: "NGO_AdminId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Users_Rehab_AdminID",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Users_ClientId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Users_NGO_AdminId",
                table: "Events");

            migrationBuilder.DropTable(
                name: "Rehab_disharges");

            migrationBuilder.DropIndex(
                name: "IX_Events_ClientId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_NGO_AdminId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Applications_Rehab_AdminID",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "Next_Of_Kins");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Client_Id",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "NGO_AdminId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "NGO_Id",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Rehab_AdminID",
                table: "Applications");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Addiction_level",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discharge_Date",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discharge_Reason",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RehabReason",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Substance_type",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Update_Comment",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Recieved_Applications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: true),
                    Processing_Date = table.Column<DateOnly>(type: "date", nullable: true),
                    Rehab_AdminId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recieved_Applications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recieved_Applications_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Recieved_Applications_Users_Rehab_AdminId",
                        column: x => x.Rehab_AdminId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SessionBookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Assignment_Date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: true),
                    NGO_AdminId = table.Column<int>(type: "int", nullable: true),
                    Social_WorkerId = table.Column<int>(type: "int", nullable: true),
                    Stime = table.Column<int>(type: "int", nullable: true),
                    Stype = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionBookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionBookings_Users_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SessionBookings_Users_NGO_AdminId",
                        column: x => x.NGO_AdminId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recieved_Applications_ApplicationId",
                table: "Recieved_Applications",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Recieved_Applications_Rehab_AdminId",
                table: "Recieved_Applications",
                column: "Rehab_AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionBookings_ClientId",
                table: "SessionBookings",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionBookings_NGO_AdminId",
                table: "SessionBookings",
                column: "NGO_AdminId");
        }
    }
}
