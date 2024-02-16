using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class Set_Cascading_Delete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_AspNetUsers_UserId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_patient_PatientId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ImportSourceFiles_AspNetUsers_UserId",
                table: "ImportSourceFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_ImportSourceFiles_pharmacy_PharmacyId",
                table: "ImportSourceFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_patientPDC_patient_PatientId",
                table: "patientPDC");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_AspNetUsers_UserId",
                table: "AuditLogs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_patient_PatientId",
                table: "AuditLogs",
                column: "PatientId",
                principalTable: "patient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ImportSourceFiles_AspNetUsers_UserId",
                table: "ImportSourceFiles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ImportSourceFiles_pharmacy_PharmacyId",
                table: "ImportSourceFiles",
                column: "PharmacyId",
                principalTable: "pharmacy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_patientPDC_patient_PatientId",
                table: "patientPDC",
                column: "PatientId",
                principalTable: "patient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_AspNetUsers_UserId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_patient_PatientId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ImportSourceFiles_AspNetUsers_UserId",
                table: "ImportSourceFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_ImportSourceFiles_pharmacy_PharmacyId",
                table: "ImportSourceFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_patientPDC_patient_PatientId",
                table: "patientPDC");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_AspNetUsers_UserId",
                table: "AuditLogs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_patient_PatientId",
                table: "AuditLogs",
                column: "PatientId",
                principalTable: "patient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ImportSourceFiles_AspNetUsers_UserId",
                table: "ImportSourceFiles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ImportSourceFiles_pharmacy_PharmacyId",
                table: "ImportSourceFiles",
                column: "PharmacyId",
                principalTable: "pharmacy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_patientPDC_patient_PatientId",
                table: "patientPDC",
                column: "PatientId",
                principalTable: "patient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
