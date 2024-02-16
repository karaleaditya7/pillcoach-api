using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class AllergyCascadeAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_allergy_patient_PatientId",
                table: "allergy");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "allergy",
                newName: "patientId");

            migrationBuilder.RenameIndex(
                name: "IX_allergy_PatientId",
                table: "allergy",
                newName: "IX_allergy_patientId");

            migrationBuilder.AddForeignKey(
                name: "FK_allergy_patient_patientId",
                table: "allergy",
                column: "patientId",
                principalTable: "patient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_allergy_patient_patientId",
                table: "allergy");

            migrationBuilder.RenameColumn(
                name: "patientId",
                table: "allergy",
                newName: "PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_allergy_patientId",
                table: "allergy",
                newName: "IX_allergy_PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_allergy_patient_PatientId",
                table: "allergy",
                column: "PatientId",
                principalTable: "patient",
                principalColumn: "Id");
        }
    }
}
