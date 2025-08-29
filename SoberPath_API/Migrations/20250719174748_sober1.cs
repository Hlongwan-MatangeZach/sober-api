using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoberPath_API.Migrations
{
    /// <inheritdoc />
    public partial class sober1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ID_Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Race = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone_Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Social_WorkerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Users_Social_WorkerId",
                        column: x => x.Social_WorkerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Addiction_level = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Substance_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: true),
                    Social_WorkerId = table.Column<int>(type: "int", nullable: true),
                    FilePaths = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Applications_Users_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Applications_Users_Social_WorkerId",
                        column: x => x.Social_WorkerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClientAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Social_WorkerId = table.Column<int>(type: "int", nullable: true),
                    NGO_AdminId = table.Column<int>(type: "int", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: true),
                    Assignment_Date = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientAssignments_Users_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientAssignments_Users_NGO_AdminId",
                        column: x => x.NGO_AdminId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientAssignments_Users_Social_WorkerId",
                        column: x => x.Social_WorkerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Next_Of_Kins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Relationship = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone_number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Next_Of_Kins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Next_Of_Kins_Users_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Rehab_Admissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: true),
                    ApplicationId = table.Column<int>(type: "int", nullable: true),
                    Admission_Date = table.Column<DateOnly>(type: "date", nullable: true),
                    Expected_Dischanrge = table.Column<DateOnly>(type: "date", nullable: true),
                    Discharged_Date = table.Column<DateOnly>(type: "date", nullable: true),
                    Dischange_status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDischarged = table.Column<bool>(type: "bit", nullable: true),
                    Rehab_AdminId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rehab_Admissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rehab_Admissions_Users_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Rehab_Admissions_Users_Rehab_AdminId",
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
                    Social_WorkerId = table.Column<int>(type: "int", nullable: true),
                    NGO_AdminId = table.Column<int>(type: "int", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: true),
                    Assignment_Date = table.Column<DateOnly>(type: "date", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: true),
                    Session_Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: true),
                    Social_WorkerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Users_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sessions_Users_Social_WorkerId",
                        column: x => x.Social_WorkerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Social_Worker_Schedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Availabillity_Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Social_WorkerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Social_Worker_Schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Social_Worker_Schedules_Users_Social_WorkerId",
                        column: x => x.Social_WorkerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Substances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Substances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Substances_Users_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Recieved_Applications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rehab_AdminId = table.Column<int>(type: "int", nullable: true),
                    ApplicationId = table.Column<int>(type: "int", nullable: true),
                    Processing_Date = table.Column<DateOnly>(type: "date", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ClientId",
                table: "Applications",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_Social_WorkerId",
                table: "Applications",
                column: "Social_WorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientAssignments_ClientId",
                table: "ClientAssignments",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientAssignments_NGO_AdminId",
                table: "ClientAssignments",
                column: "NGO_AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientAssignments_Social_WorkerId",
                table: "ClientAssignments",
                column: "Social_WorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_Next_Of_Kins_ClientId",
                table: "Next_Of_Kins",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Recieved_Applications_ApplicationId",
                table: "Recieved_Applications",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Recieved_Applications_Rehab_AdminId",
                table: "Recieved_Applications",
                column: "Rehab_AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Rehab_Admissions_ClientId",
                table: "Rehab_Admissions",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Rehab_Admissions_Rehab_AdminId",
                table: "Rehab_Admissions",
                column: "Rehab_AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionBookings_ClientId",
                table: "SessionBookings",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionBookings_NGO_AdminId",
                table: "SessionBookings",
                column: "NGO_AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_ClientId",
                table: "Sessions",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_Social_WorkerId",
                table: "Sessions",
                column: "Social_WorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_Social_Worker_Schedules_Social_WorkerId",
                table: "Social_Worker_Schedules",
                column: "Social_WorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_Substances_ClientId",
                table: "Substances",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Social_WorkerId",
                table: "Users",
                column: "Social_WorkerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientAssignments");

            migrationBuilder.DropTable(
                name: "Next_Of_Kins");

            migrationBuilder.DropTable(
                name: "Recieved_Applications");

            migrationBuilder.DropTable(
                name: "Rehab_Admissions");

            migrationBuilder.DropTable(
                name: "SessionBookings");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Social_Worker_Schedules");

            migrationBuilder.DropTable(
                name: "Substances");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
