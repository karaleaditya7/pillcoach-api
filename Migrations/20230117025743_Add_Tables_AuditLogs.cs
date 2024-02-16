using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class Add_Tables_AuditLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditActionSourceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionSourceType = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditActionSourceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditActionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionType = table.Column<string>(type: "varchar(30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditActionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LogDateUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    ActionTypeId = table.Column<int>(type: "int", nullable: false),
                    ActionSourceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditLogs_AuditActionSourceTypes_ActionSourceId",
                        column: x => x.ActionSourceId,
                        principalTable: "AuditActionSourceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditLogs_AuditActionTypes_ActionTypeId",
                        column: x => x.ActionTypeId,
                        principalTable: "AuditActionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditLogs_patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AuditActionSourceTypes",
                columns: new[] { "Id", "ActionSourceType" },
                values: new object[,]
                {
                    { 1, "PatientProfile" },
                    { 2, "PatientReportedMedications" },
                    { 3, "HealthConditions" },
                    { 4, "PatientInbox" },
                    { 5, "CompanyInbox" },
                    { 6, "RefillRequest" },
                    { 7, "DoctorPhone" },
                    { 8, "DoctorFax" },
                    { 9, "PharmacyCall" },
                    { 10, "PatientPhone" },
                    { 11, "PatientEmail" },
                    { 12, "Appointments" },
                    { 13, "PatientDocument" },
                    { 14, "PatientNote" },
                    { 15, "PharmacyNote" }
                });

            migrationBuilder.InsertData(
                table: "AuditActionTypes",
                columns: new[] { "Id", "ActionType" },
                values: new object[,]
                {
                    { 1, "Access" },
                    { 2, "Exit" },
                    { 3, "Create" },
                    { 4, "Delete" },
                    { 5, "Modify" },
                    { 6, "Texted" },
                    { 7, "Emailed" },
                    { 8, "Mailed" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ActionSourceId",
                table: "AuditLogs",
                column: "ActionSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ActionTypeId",
                table: "AuditLogs",
                column: "ActionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_PatientId",
                table: "AuditLogs",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "AuditActionSourceTypes");

            migrationBuilder.DropTable(
                name: "AuditActionTypes");
        }
    }
}
