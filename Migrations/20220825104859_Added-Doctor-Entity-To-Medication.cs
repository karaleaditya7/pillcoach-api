using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class AddedDoctorEntityToMedication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DoctorPrescribedId",
                table: "medication",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_medication_DoctorPrescribedId",
                table: "medication",
                column: "DoctorPrescribedId");

            migrationBuilder.AddForeignKey(
                name: "FK_medication_doctor_DoctorPrescribedId",
                table: "medication",
                column: "DoctorPrescribedId",
                principalTable: "doctor",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_medication_doctor_DoctorPrescribedId",
                table: "medication");

            migrationBuilder.DropIndex(
                name: "IX_medication_DoctorPrescribedId",
                table: "medication");

            migrationBuilder.DropColumn(
                name: "DoctorPrescribedId",
                table: "medication");
        }
    }
}
