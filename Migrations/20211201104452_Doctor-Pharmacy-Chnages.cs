using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class DoctorPharmacyChnages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_doctorPharmacy_doctor_doctorId",
                table: "doctorPharmacy");

            migrationBuilder.DropForeignKey(
                name: "FK_doctorPharmacy_pharmacy_pharmacyId",
                table: "doctorPharmacy");

            migrationBuilder.RenameColumn(
                name: "pharmacyId",
                table: "doctorPharmacy",
                newName: "pharmaciesId");

            migrationBuilder.RenameColumn(
                name: "doctorId",
                table: "doctorPharmacy",
                newName: "doctorsId");

            migrationBuilder.RenameIndex(
                name: "IX_doctorPharmacy_pharmacyId",
                table: "doctorPharmacy",
                newName: "IX_doctorPharmacy_pharmaciesId");

            migrationBuilder.RenameIndex(
                name: "IX_doctorPharmacy_doctorId",
                table: "doctorPharmacy",
                newName: "IX_doctorPharmacy_doctorsId");

            migrationBuilder.AddForeignKey(
                name: "FK_doctorPharmacy_doctor_doctorsId",
                table: "doctorPharmacy",
                column: "doctorsId",
                principalTable: "doctor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_doctorPharmacy_pharmacy_pharmaciesId",
                table: "doctorPharmacy",
                column: "pharmaciesId",
                principalTable: "pharmacy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_doctorPharmacy_doctor_doctorsId",
                table: "doctorPharmacy");

            migrationBuilder.DropForeignKey(
                name: "FK_doctorPharmacy_pharmacy_pharmaciesId",
                table: "doctorPharmacy");

            migrationBuilder.RenameColumn(
                name: "pharmaciesId",
                table: "doctorPharmacy",
                newName: "pharmacyId");

            migrationBuilder.RenameColumn(
                name: "doctorsId",
                table: "doctorPharmacy",
                newName: "doctorId");

            migrationBuilder.RenameIndex(
                name: "IX_doctorPharmacy_pharmaciesId",
                table: "doctorPharmacy",
                newName: "IX_doctorPharmacy_pharmacyId");

            migrationBuilder.RenameIndex(
                name: "IX_doctorPharmacy_doctorsId",
                table: "doctorPharmacy",
                newName: "IX_doctorPharmacy_doctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_doctorPharmacy_doctor_doctorId",
                table: "doctorPharmacy",
                column: "doctorId",
                principalTable: "doctor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_doctorPharmacy_pharmacy_pharmacyId",
                table: "doctorPharmacy",
                column: "pharmacyId",
                principalTable: "pharmacy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
