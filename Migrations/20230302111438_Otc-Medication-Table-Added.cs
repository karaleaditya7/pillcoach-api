using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class OtcMedicationTableAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "otcMedication",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    direction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DoctorPrescribedId = table.Column<int>(type: "int", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    condition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sbdcName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    gpckName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_otcMedication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_otcMedication_doctor_DoctorPrescribedId",
                        column: x => x.DoctorPrescribedId,
                        principalTable: "doctor",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_otcMedication_patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patient",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_otcMedication_DoctorPrescribedId",
                table: "otcMedication",
                column: "DoctorPrescribedId");

            migrationBuilder.CreateIndex(
                name: "IX_otcMedication_PatientId",
                table: "otcMedication",
                column: "PatientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "otcMedication");
        }
    }
}
