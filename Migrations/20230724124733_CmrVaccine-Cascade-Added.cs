using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class CmrVaccineCascadeAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cmrVaccine_serviceTakeawayInformation_ServiceTakeawayInformationId",
                table: "cmrVaccine");

            migrationBuilder.RenameColumn(
                name: "ServiceTakeawayInformationId",
                table: "cmrVaccine",
                newName: "servicetakeawaynformationId");

            migrationBuilder.RenameIndex(
                name: "IX_cmrVaccine_ServiceTakeawayInformationId",
                table: "cmrVaccine",
                newName: "IX_cmrVaccine_servicetakeawaynformationId");

            migrationBuilder.AddForeignKey(
                name: "FK_cmrVaccine_serviceTakeawayInformation_servicetakeawaynformationId",
                table: "cmrVaccine",
                column: "servicetakeawaynformationId",
                principalTable: "serviceTakeawayInformation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cmrVaccine_serviceTakeawayInformation_servicetakeawaynformationId",
                table: "cmrVaccine");

            migrationBuilder.RenameColumn(
                name: "servicetakeawaynformationId",
                table: "cmrVaccine",
                newName: "ServiceTakeawayInformationId");

            migrationBuilder.RenameIndex(
                name: "IX_cmrVaccine_servicetakeawaynformationId",
                table: "cmrVaccine",
                newName: "IX_cmrVaccine_ServiceTakeawayInformationId");

            migrationBuilder.AddForeignKey(
                name: "FK_cmrVaccine_serviceTakeawayInformation_ServiceTakeawayInformationId",
                table: "cmrVaccine",
                column: "ServiceTakeawayInformationId",
                principalTable: "serviceTakeawayInformation",
                principalColumn: "Id");
        }
    }
}
