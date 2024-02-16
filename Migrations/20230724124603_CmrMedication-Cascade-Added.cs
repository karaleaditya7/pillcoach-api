using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class CmrMedicationCascadeAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cmrMedication_patient_PatientId",
                table: "cmrMedication");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "cmrMedication",
                newName: "patientId");

            migrationBuilder.RenameIndex(
                name: "IX_cmrMedication_PatientId",
                table: "cmrMedication",
                newName: "IX_cmrMedication_patientId");

            migrationBuilder.AddForeignKey(
                name: "FK_cmrMedication_patient_patientId",
                table: "cmrMedication",
                column: "patientId",
                principalTable: "patient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cmrMedication_patient_patientId",
                table: "cmrMedication");

            migrationBuilder.RenameColumn(
                name: "patientId",
                table: "cmrMedication",
                newName: "PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_cmrMedication_patientId",
                table: "cmrMedication",
                newName: "IX_cmrMedication_PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_cmrMedication_patient_PatientId",
                table: "cmrMedication",
                column: "PatientId",
                principalTable: "patient",
                principalColumn: "Id");
        }
    }
}
