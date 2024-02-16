using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class AppointmentCascadeAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointment_AspNetUsers_UserId",
                table: "appointment");

            migrationBuilder.DropForeignKey(
                name: "FK_appointment_patient_PatientId",
                table: "appointment");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "appointment",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "appointment",
                newName: "patientId");

            migrationBuilder.RenameIndex(
                name: "IX_appointment_UserId",
                table: "appointment",
                newName: "IX_appointment_userId");

            migrationBuilder.RenameIndex(
                name: "IX_appointment_PatientId",
                table: "appointment",
                newName: "IX_appointment_patientId");

            migrationBuilder.AddForeignKey(
                name: "FK_appointment_AspNetUsers_userId",
                table: "appointment",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_appointment_patient_patientId",
                table: "appointment",
                column: "patientId",
                principalTable: "patient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointment_AspNetUsers_userId",
                table: "appointment");

            migrationBuilder.DropForeignKey(
                name: "FK_appointment_patient_patientId",
                table: "appointment");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "appointment",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "patientId",
                table: "appointment",
                newName: "PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_appointment_userId",
                table: "appointment",
                newName: "IX_appointment_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_appointment_patientId",
                table: "appointment",
                newName: "IX_appointment_PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_appointment_AspNetUsers_UserId",
                table: "appointment",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_appointment_patient_PatientId",
                table: "appointment",
                column: "PatientId",
                principalTable: "patient",
                principalColumn: "Id");
        }
    }
}
