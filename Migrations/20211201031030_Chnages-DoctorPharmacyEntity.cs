using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class ChnagesDoctorPharmacyEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("IF OBJECT_ID('doctorPharmacy', 'U') IS NOT NULL DROP TABLE doctorPharmacy");
            migrationBuilder.CreateTable(
                name: "doctorPharmacy",
                columns: table => new
                {
                    doctorId = table.Column<int>(type: "int", nullable: false),
                    pharmacyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_doctorPharmacy_doctor_doctorId",
                        column: x => x.doctorId,
                        principalTable: "doctor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_doctorPharmacy_pharmacy_pharmacyId",
                        column: x => x.pharmacyId,
                        principalTable: "pharmacy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_doctorPharmacy_doctorId",
                table: "doctorPharmacy",
                column: "doctorId");

            migrationBuilder.CreateIndex(
                name: "IX_doctorPharmacy_pharmacyId",
                table: "doctorPharmacy",
                column: "pharmacyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "doctorPharmacy");
        }
    }
}
