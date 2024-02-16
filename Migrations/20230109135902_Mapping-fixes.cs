using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class Mappingfixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_allergy_medicationSubstance_medicationSubstanceId",
                table: "allergy");

            migrationBuilder.DropForeignKey(
                name: "FK_sideEffect_medicationSubstance_medicationSubstanceId",
                table: "sideEffect");

            migrationBuilder.DropIndex(
                name: "IX_sideEffect_medicationSubstanceId",
                table: "sideEffect");

            migrationBuilder.DropIndex(
                name: "IX_allergy_medicationSubstanceId",
                table: "allergy");

            migrationBuilder.RenameColumn(
                name: "medicationSubstanceId",
                table: "sideEffect",
                newName: "MedicationSubstanceId");

            migrationBuilder.RenameColumn(
                name: "medicationSubstanceId",
                table: "allergy",
                newName: "MedicationSubstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_sideEffect_MedicationSubstanceId",
                table: "sideEffect",
                column: "MedicationSubstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_allergy_MedicationSubstanceId",
                table: "allergy",
                column: "MedicationSubstanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_allergy_medicationSubstance_MedicationSubstanceId",
                table: "allergy",
                column: "MedicationSubstanceId",
                principalTable: "medicationSubstance",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_sideEffect_medicationSubstance_MedicationSubstanceId",
                table: "sideEffect",
                column: "MedicationSubstanceId",
                principalTable: "medicationSubstance",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_allergy_medicationSubstance_MedicationSubstanceId",
                table: "allergy");

            migrationBuilder.DropForeignKey(
                name: "FK_sideEffect_medicationSubstance_MedicationSubstanceId",
                table: "sideEffect");

            migrationBuilder.DropIndex(
                name: "IX_sideEffect_MedicationSubstanceId",
                table: "sideEffect");

            migrationBuilder.DropIndex(
                name: "IX_allergy_MedicationSubstanceId",
                table: "allergy");

            migrationBuilder.RenameColumn(
                name: "MedicationSubstanceId",
                table: "sideEffect",
                newName: "medicationSubstanceId");

            migrationBuilder.RenameColumn(
                name: "MedicationSubstanceId",
                table: "allergy",
                newName: "medicationSubstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_sideEffect_medicationSubstanceId",
                table: "sideEffect",
                column: "medicationSubstanceId",
                unique: true,
                filter: "[medicationSubstanceId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_allergy_medicationSubstanceId",
                table: "allergy",
                column: "medicationSubstanceId",
                unique: true,
                filter: "[medicationSubstanceId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_allergy_medicationSubstance_medicationSubstanceId",
                table: "allergy",
                column: "medicationSubstanceId",
                principalTable: "medicationSubstance",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_sideEffect_medicationSubstance_medicationSubstanceId",
                table: "sideEffect",
                column: "medicationSubstanceId",
                principalTable: "medicationSubstance",
                principalColumn: "Id");
        }
    }
}
