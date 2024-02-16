using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class ServiceTakeawayInformationCascadeAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_serviceTakeawayInformation_patient_PatientId",
                table: "serviceTakeawayInformation");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "serviceTakeawayInformation",
                newName: "patientId");

            migrationBuilder.RenameIndex(
                name: "IX_serviceTakeawayInformation_PatientId",
                table: "serviceTakeawayInformation",
                newName: "IX_serviceTakeawayInformation_patientId");

            migrationBuilder.AddForeignKey(
                name: "FK_serviceTakeawayInformation_patient_patientId",
                table: "serviceTakeawayInformation",
                column: "patientId",
                principalTable: "patient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_serviceTakeawayInformation_patient_patientId",
                table: "serviceTakeawayInformation");

            migrationBuilder.RenameColumn(
                name: "patientId",
                table: "serviceTakeawayInformation",
                newName: "PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_serviceTakeawayInformation_patientId",
                table: "serviceTakeawayInformation",
                newName: "IX_serviceTakeawayInformation_PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_serviceTakeawayInformation_patient_PatientId",
                table: "serviceTakeawayInformation",
                column: "PatientId",
                principalTable: "patient",
                principalColumn: "Id");
        }
    }
}
