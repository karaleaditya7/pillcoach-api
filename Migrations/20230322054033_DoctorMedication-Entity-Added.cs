using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class DoctorMedicationEntityAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "doctorMedication",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoctorPrescribedId = table.Column<int>(type: "int", nullable: true),
                    CmrMedicationId = table.Column<int>(type: "int", nullable: true),
                    MedicationId = table.Column<int>(type: "int", nullable: true),
                    OtcMedicationId = table.Column<int>(type: "int", nullable: true),
                    MedicationReconciliationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doctorMedication", x => x.id);
                    table.ForeignKey(
                        name: "FK_doctorMedication_cmrMedication_CmrMedicationId",
                        column: x => x.CmrMedicationId,
                        principalTable: "cmrMedication",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_doctorMedication_doctor_DoctorPrescribedId",
                        column: x => x.DoctorPrescribedId,
                        principalTable: "doctor",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_doctorMedication_medication_MedicationId",
                        column: x => x.MedicationId,
                        principalTable: "medication",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_doctorMedication_medicationReconciliation_MedicationReconciliationId",
                        column: x => x.MedicationReconciliationId,
                        principalTable: "medicationReconciliation",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_doctorMedication_otcMedication_OtcMedicationId",
                        column: x => x.OtcMedicationId,
                        principalTable: "otcMedication",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_doctorMedication_CmrMedicationId",
                table: "doctorMedication",
                column: "CmrMedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_doctorMedication_DoctorPrescribedId",
                table: "doctorMedication",
                column: "DoctorPrescribedId");

            migrationBuilder.CreateIndex(
                name: "IX_doctorMedication_MedicationId",
                table: "doctorMedication",
                column: "MedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_doctorMedication_MedicationReconciliationId",
                table: "doctorMedication",
                column: "MedicationReconciliationId");

            migrationBuilder.CreateIndex(
                name: "IX_doctorMedication_OtcMedicationId",
                table: "doctorMedication",
                column: "OtcMedicationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "doctorMedication");
        }
    }
}
