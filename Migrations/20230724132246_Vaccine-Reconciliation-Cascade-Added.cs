using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class VaccineReconciliationCascadeAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_vaccineReconciliation_serviceTakeAwayMedReconciliation_ServiceTakeAwayMedReconciliationId",
                table: "vaccineReconciliation");

            migrationBuilder.RenameColumn(
                name: "ServiceTakeAwayMedReconciliationId",
                table: "vaccineReconciliation",
                newName: "serviceTakeAwayMedReconciliationId");

            migrationBuilder.RenameIndex(
                name: "IX_vaccineReconciliation_ServiceTakeAwayMedReconciliationId",
                table: "vaccineReconciliation",
                newName: "IX_vaccineReconciliation_serviceTakeAwayMedReconciliationId");

            migrationBuilder.AddForeignKey(
                name: "FK_vaccineReconciliation_serviceTakeAwayMedReconciliation_serviceTakeAwayMedReconciliationId",
                table: "vaccineReconciliation",
                column: "serviceTakeAwayMedReconciliationId",
                principalTable: "serviceTakeAwayMedReconciliation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_vaccineReconciliation_serviceTakeAwayMedReconciliation_serviceTakeAwayMedReconciliationId",
                table: "vaccineReconciliation");

            migrationBuilder.RenameColumn(
                name: "serviceTakeAwayMedReconciliationId",
                table: "vaccineReconciliation",
                newName: "ServiceTakeAwayMedReconciliationId");

            migrationBuilder.RenameIndex(
                name: "IX_vaccineReconciliation_serviceTakeAwayMedReconciliationId",
                table: "vaccineReconciliation",
                newName: "IX_vaccineReconciliation_ServiceTakeAwayMedReconciliationId");

            migrationBuilder.AddForeignKey(
                name: "FK_vaccineReconciliation_serviceTakeAwayMedReconciliation_ServiceTakeAwayMedReconciliationId",
                table: "vaccineReconciliation",
                column: "ServiceTakeAwayMedReconciliationId",
                principalTable: "serviceTakeAwayMedReconciliation",
                principalColumn: "Id");
        }
    }
}
