using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class NonRelatedMedicationToDoCascadeAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_nonRelatedMedicationToDo_patient_PatientId",
                table: "nonRelatedMedicationToDo");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "nonRelatedMedicationToDo",
                newName: "patientId");

            migrationBuilder.RenameIndex(
                name: "IX_nonRelatedMedicationToDo_PatientId",
                table: "nonRelatedMedicationToDo",
                newName: "IX_nonRelatedMedicationToDo_patientId");

            migrationBuilder.AddForeignKey(
                name: "FK_nonRelatedMedicationToDo_patient_patientId",
                table: "nonRelatedMedicationToDo",
                column: "patientId",
                principalTable: "patient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_nonRelatedMedicationToDo_patient_patientId",
                table: "nonRelatedMedicationToDo");

            migrationBuilder.RenameColumn(
                name: "patientId",
                table: "nonRelatedMedicationToDo",
                newName: "PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_nonRelatedMedicationToDo_patientId",
                table: "nonRelatedMedicationToDo",
                newName: "IX_nonRelatedMedicationToDo_PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_nonRelatedMedicationToDo_patient_PatientId",
                table: "nonRelatedMedicationToDo",
                column: "PatientId",
                principalTable: "patient",
                principalColumn: "Id");
        }
    }
}
