using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class ServiceMedRecTakeawayInfoCascadeAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_serviceTakeAwayMedReconciliation_patient_PatientId",
                table: "serviceTakeAwayMedReconciliation");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "serviceTakeAwayMedReconciliation",
                newName: "patientId");

            migrationBuilder.RenameIndex(
                name: "IX_serviceTakeAwayMedReconciliation_PatientId",
                table: "serviceTakeAwayMedReconciliation",
                newName: "IX_serviceTakeAwayMedReconciliation_patientId");

            migrationBuilder.AddForeignKey(
                name: "FK_serviceTakeAwayMedReconciliation_patient_patientId",
                table: "serviceTakeAwayMedReconciliation",
                column: "patientId",
                principalTable: "patient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_serviceTakeAwayMedReconciliation_patient_patientId",
                table: "serviceTakeAwayMedReconciliation");

            migrationBuilder.RenameColumn(
                name: "patientId",
                table: "serviceTakeAwayMedReconciliation",
                newName: "PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_serviceTakeAwayMedReconciliation_patientId",
                table: "serviceTakeAwayMedReconciliation",
                newName: "IX_serviceTakeAwayMedReconciliation_PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_serviceTakeAwayMedReconciliation_patient_PatientId",
                table: "serviceTakeAwayMedReconciliation",
                column: "PatientId",
                principalTable: "patient",
                principalColumn: "Id");
        }
    }
}
