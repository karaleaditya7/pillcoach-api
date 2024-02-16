using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class AddedAppointmentEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "appointment",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    startDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    duration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    isCancel = table.Column<bool>(type: "bit", nullable: false),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointment", x => x.ID);
                    table.ForeignKey(
                        name: "FK_appointment_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_appointment_patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patient",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_appointment_PatientId",
                table: "appointment",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_appointment_UserId",
                table: "appointment",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appointment");
        }
    }
}
