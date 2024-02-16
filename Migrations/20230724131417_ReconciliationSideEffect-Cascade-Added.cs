using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class ReconciliationSideEffectCascadeAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reconciliationSideeffect_patient_PatientId",
                table: "reconciliationSideeffect");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "reconciliationSideeffect",
                newName: "patientId");

            migrationBuilder.RenameIndex(
                name: "IX_reconciliationSideeffect_PatientId",
                table: "reconciliationSideeffect",
                newName: "IX_reconciliationSideeffect_patientId");

            migrationBuilder.AddForeignKey(
                name: "FK_reconciliationSideeffect_patient_patientId",
                table: "reconciliationSideeffect",
                column: "patientId",
                principalTable: "patient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reconciliationSideeffect_patient_patientId",
                table: "reconciliationSideeffect");

            migrationBuilder.RenameColumn(
                name: "patientId",
                table: "reconciliationSideeffect",
                newName: "PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_reconciliationSideeffect_patientId",
                table: "reconciliationSideeffect",
                newName: "IX_reconciliationSideeffect_PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_reconciliationSideeffect_patient_PatientId",
                table: "reconciliationSideeffect",
                column: "PatientId",
                principalTable: "patient",
                principalColumn: "Id");
        }
    }
}
