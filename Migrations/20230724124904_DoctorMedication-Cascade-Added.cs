using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class DoctorMedicationCascadeAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_doctorMedication_cmrMedication_CmrMedicationId",
                table: "doctorMedication");

            migrationBuilder.DropForeignKey(
                name: "FK_doctorMedication_medication_MedicationId",
                table: "doctorMedication");

            migrationBuilder.DropForeignKey(
                name: "FK_doctorMedication_medicationReconciliation_MedicationReconciliationId",
                table: "doctorMedication");

            migrationBuilder.DropForeignKey(
                name: "FK_doctorMedication_otcMedication_OtcMedicationId",
                table: "doctorMedication");

            migrationBuilder.RenameColumn(
                name: "OtcMedicationId",
                table: "doctorMedication",
                newName: "otcMedicationId");

            migrationBuilder.RenameColumn(
                name: "MedicationReconciliationId",
                table: "doctorMedication",
                newName: "medicationReconciliationId");

            migrationBuilder.RenameColumn(
                name: "MedicationId",
                table: "doctorMedication",
                newName: "medicationId");

            migrationBuilder.RenameColumn(
                name: "CmrMedicationId",
                table: "doctorMedication",
                newName: "cmrMedicationId");

            migrationBuilder.RenameIndex(
                name: "IX_doctorMedication_OtcMedicationId",
                table: "doctorMedication",
                newName: "IX_doctorMedication_otcMedicationId");

            migrationBuilder.RenameIndex(
                name: "IX_doctorMedication_MedicationReconciliationId",
                table: "doctorMedication",
                newName: "IX_doctorMedication_medicationReconciliationId");

            migrationBuilder.RenameIndex(
                name: "IX_doctorMedication_MedicationId",
                table: "doctorMedication",
                newName: "IX_doctorMedication_medicationId");

            migrationBuilder.RenameIndex(
                name: "IX_doctorMedication_CmrMedicationId",
                table: "doctorMedication",
                newName: "IX_doctorMedication_cmrMedicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_doctorMedication_cmrMedication_cmrMedicationId",
                table: "doctorMedication",
                column: "cmrMedicationId",
                principalTable: "cmrMedication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_doctorMedication_medication_medicationId",
                table: "doctorMedication",
                column: "medicationId",
                principalTable: "medication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_doctorMedication_medicationReconciliation_medicationReconciliationId",
                table: "doctorMedication",
                column: "medicationReconciliationId",
                principalTable: "medicationReconciliation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_doctorMedication_otcMedication_otcMedicationId",
                table: "doctorMedication",
                column: "otcMedicationId",
                principalTable: "otcMedication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_doctorMedication_cmrMedication_cmrMedicationId",
                table: "doctorMedication");

            migrationBuilder.DropForeignKey(
                name: "FK_doctorMedication_medication_medicationId",
                table: "doctorMedication");

            migrationBuilder.DropForeignKey(
                name: "FK_doctorMedication_medicationReconciliation_medicationReconciliationId",
                table: "doctorMedication");

            migrationBuilder.DropForeignKey(
                name: "FK_doctorMedication_otcMedication_otcMedicationId",
                table: "doctorMedication");

            migrationBuilder.RenameColumn(
                name: "otcMedicationId",
                table: "doctorMedication",
                newName: "OtcMedicationId");

            migrationBuilder.RenameColumn(
                name: "medicationReconciliationId",
                table: "doctorMedication",
                newName: "MedicationReconciliationId");

            migrationBuilder.RenameColumn(
                name: "medicationId",
                table: "doctorMedication",
                newName: "MedicationId");

            migrationBuilder.RenameColumn(
                name: "cmrMedicationId",
                table: "doctorMedication",
                newName: "CmrMedicationId");

            migrationBuilder.RenameIndex(
                name: "IX_doctorMedication_otcMedicationId",
                table: "doctorMedication",
                newName: "IX_doctorMedication_OtcMedicationId");

            migrationBuilder.RenameIndex(
                name: "IX_doctorMedication_medicationReconciliationId",
                table: "doctorMedication",
                newName: "IX_doctorMedication_MedicationReconciliationId");

            migrationBuilder.RenameIndex(
                name: "IX_doctorMedication_medicationId",
                table: "doctorMedication",
                newName: "IX_doctorMedication_MedicationId");

            migrationBuilder.RenameIndex(
                name: "IX_doctorMedication_cmrMedicationId",
                table: "doctorMedication",
                newName: "IX_doctorMedication_CmrMedicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_doctorMedication_cmrMedication_CmrMedicationId",
                table: "doctorMedication",
                column: "CmrMedicationId",
                principalTable: "cmrMedication",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_doctorMedication_medication_MedicationId",
                table: "doctorMedication",
                column: "MedicationId",
                principalTable: "medication",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_doctorMedication_medicationReconciliation_MedicationReconciliationId",
                table: "doctorMedication",
                column: "MedicationReconciliationId",
                principalTable: "medicationReconciliation",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_doctorMedication_otcMedication_OtcMedicationId",
                table: "doctorMedication",
                column: "OtcMedicationId",
                principalTable: "otcMedication",
                principalColumn: "Id");
        }
    }
}
