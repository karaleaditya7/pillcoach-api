using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class ReconciliationAllergyCascadeAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reconciliationAllergy_patient_PatientId",
                table: "reconciliationAllergy");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "reconciliationAllergy",
                newName: "patientId");

            migrationBuilder.RenameIndex(
                name: "IX_reconciliationAllergy_PatientId",
                table: "reconciliationAllergy",
                newName: "IX_reconciliationAllergy_patientId");

            migrationBuilder.AddForeignKey(
                name: "FK_reconciliationAllergy_patient_patientId",
                table: "reconciliationAllergy",
                column: "patientId",
                principalTable: "patient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reconciliationAllergy_patient_patientId",
                table: "reconciliationAllergy");

            migrationBuilder.RenameColumn(
                name: "patientId",
                table: "reconciliationAllergy",
                newName: "PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_reconciliationAllergy_patientId",
                table: "reconciliationAllergy",
                newName: "IX_reconciliationAllergy_PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_reconciliationAllergy_patient_PatientId",
                table: "reconciliationAllergy",
                column: "PatientId",
                principalTable: "patient",
                principalColumn: "Id");
        }
    }
}
