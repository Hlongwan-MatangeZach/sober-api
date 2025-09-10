using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoberPath_API.Migrations
{
    /// <inheritdoc />
    public partial class thoba : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rehabilitation_Progresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Progress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rehabilitation_Progresses", x => x.Id);
                });

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
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: true),
                    Social_WorkerId = table.Column<int>(type: "int", nullable: true),
                    Rehab_AdminID = table.Column<int>(type: "int", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Status_Update_Date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HasRelapse = table.Column<bool>(type: "bit", nullable: true)
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
                        name: "FK_Applications_Users_Rehab_AdminID",
                        column: x => x.Rehab_AdminID,
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
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Social_Id = table.Column<int>(type: "int", nullable: true),
                    Client_Id = table.Column<int>(type: "int", nullable: true),
                    NGO_Id = table.Column<int>(type: "int", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: true),
                    NGO_AdminId = table.Column<int>(type: "int", nullable: true),
                    Social_WorkerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_Users_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Events_Users_NGO_AdminId",
                        column: x => x.NGO_AdminId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Events_Users_Social_WorkerId",
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
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                name: "rooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BuildingName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoomNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllocatedDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rooms_Users_ClientId",
                        column: x => x.ClientId,
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
                    ClientId = table.Column<int>(type: "int", nullable: true),
                    DailyThreshold = table.Column<double>(type: "float", nullable: true),
                    unit = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_Rehab_disharges_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Records",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecordedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Quantity = table.Column<double>(type: "float", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: true),
                    SubstanceId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Records", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Records_Substances_SubstanceId",
                        column: x => x.SubstanceId,
                        principalTable: "Substances",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Records_Users_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ClientId",
                table: "Applications",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_Rehab_AdminID",
                table: "Applications",
                column: "Rehab_AdminID");

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
                name: "IX_Events_ClientId",
                table: "Events",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_NGO_AdminId",
                table: "Events",
                column: "NGO_AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_Social_WorkerId",
                table: "Events",
                column: "Social_WorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_Next_Of_Kins_ClientId",
                table: "Next_Of_Kins",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Records_ClientId",
                table: "Records",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Records_SubstanceId",
                table: "Records",
                column: "SubstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Rehab_Admissions_ClientId",
                table: "Rehab_Admissions",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Rehab_Admissions_Rehab_AdminId",
                table: "Rehab_Admissions",
                column: "Rehab_AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Rehab_disharges_ApplicationId",
                table: "Rehab_disharges",
                column: "ApplicationId",
                unique: true,
                filter: "[ApplicationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_rooms_ClientId",
                table: "rooms",
                column: "ClientId",
                unique: true,
                filter: "[ClientId] IS NOT NULL");

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
                name: "Events");

            migrationBuilder.DropTable(
                name: "Next_Of_Kins");

            migrationBuilder.DropTable(
                name: "Records");

            migrationBuilder.DropTable(
                name: "Rehab_Admissions");

            migrationBuilder.DropTable(
                name: "Rehab_disharges");

            migrationBuilder.DropTable(
                name: "Rehabilitation_Progresses");

            migrationBuilder.DropTable(
                name: "rooms");

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
