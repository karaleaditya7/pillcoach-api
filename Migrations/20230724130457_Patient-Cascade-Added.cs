using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class PatientCascadeAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_patient_pharmacy_PharmacyId",
                table: "patient");

            migrationBuilder.RenameColumn(
                name: "PharmacyId",
                table: "patient",
                newName: "pharmacyId");

            migrationBuilder.RenameIndex(
                name: "IX_patient_PharmacyId",
                table: "patient",
                newName: "IX_patient_pharmacyId");

            migrationBuilder.AddColumn<int>(
                name: "PharmacyId1",
                table: "patient",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_patient_PharmacyId1",
                table: "patient",
                column: "PharmacyId1");

            migrationBuilder.AddForeignKey(
                name: "FK_patient_pharmacy_pharmacyId",
                table: "patient",
                column: "pharmacyId",
                principalTable: "pharmacy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_patient_pharmacy_PharmacyId1",
                table: "patient",
                column: "PharmacyId1",
                principalTable: "pharmacy",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_patient_pharmacy_pharmacyId",
                table: "patient");

            migrationBuilder.DropForeignKey(
                name: "FK_patient_pharmacy_PharmacyId1",
                table: "patient");

            migrationBuilder.DropIndex(
                name: "IX_patient_PharmacyId1",
                table: "patient");

            migrationBuilder.DropColumn(
                name: "PharmacyId1",
                table: "patient");

            migrationBuilder.RenameColumn(
                name: "pharmacyId",
                table: "patient",
                newName: "PharmacyId");

            migrationBuilder.RenameIndex(
                name: "IX_patient_pharmacyId",
                table: "patient",
                newName: "IX_patient_PharmacyId");

            migrationBuilder.AddForeignKey(
                name: "FK_patient_pharmacy_PharmacyId",
                table: "patient",
                column: "PharmacyId",
                principalTable: "pharmacy",
                principalColumn: "Id");
        }
    }
}
