using Microsoft.EntityFrameworkCore.Migrations;

namespace OntrackDb.Migrations
{
    public partial class foreignkeyrelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PatientVendorRxID",
                table: "patient",
                newName: "patientVendorRxID");

            migrationBuilder.CreateTable(
                name: "PharmacyUsers",
                columns: table => new
                {
                    PharmacyId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PharmacyUsers", x => new { x.UserId, x.PharmacyId });
                    table.ForeignKey(
                        name: "FK_PharmacyUsers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PharmacyUsers_pharmacy_PharmacyId",
                        column: x => x.PharmacyId,
                        principalTable: "pharmacy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PharmacyUsers_PharmacyId",
                table: "PharmacyUsers",
                column: "PharmacyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PharmacyUsers");

            migrationBuilder.RenameColumn(
                name: "patientVendorRxID",
                table: "patient",
                newName: "PatientVendorRxID");
        }
    }
}
