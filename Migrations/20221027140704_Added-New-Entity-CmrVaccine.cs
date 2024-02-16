using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class AddedNewEntityCmrVaccine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppointmentID",
                table: "serviceTakeawayInformation",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cmrReceiveTypeFirstName",
                table: "serviceTakeawayInformation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cmrReceiveTypeLastName",
                table: "serviceTakeawayInformation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isAttached",
                table: "cmrMedication",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "cmrVaccine",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceTakeawayInformationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cmrVaccine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cmrVaccine_serviceTakeawayInformation_ServiceTakeawayInformationId",
                        column: x => x.ServiceTakeawayInformationId,
                        principalTable: "serviceTakeawayInformation",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_serviceTakeawayInformation_AppointmentID",
                table: "serviceTakeawayInformation",
                column: "AppointmentID");

            migrationBuilder.CreateIndex(
                name: "IX_cmrVaccine_ServiceTakeawayInformationId",
                table: "cmrVaccine",
                column: "ServiceTakeawayInformationId");

            migrationBuilder.AddForeignKey(
                name: "FK_serviceTakeawayInformation_appointment_AppointmentID",
                table: "serviceTakeawayInformation",
                column: "AppointmentID",
                principalTable: "appointment",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_serviceTakeawayInformation_appointment_AppointmentID",
                table: "serviceTakeawayInformation");

            migrationBuilder.DropTable(
                name: "cmrVaccine");

            migrationBuilder.DropIndex(
                name: "IX_serviceTakeawayInformation_AppointmentID",
                table: "serviceTakeawayInformation");

            migrationBuilder.DropColumn(
                name: "AppointmentID",
                table: "serviceTakeawayInformation");

            migrationBuilder.DropColumn(
                name: "cmrReceiveTypeFirstName",
                table: "serviceTakeawayInformation");

            migrationBuilder.DropColumn(
                name: "cmrReceiveTypeLastName",
                table: "serviceTakeawayInformation");

            migrationBuilder.DropColumn(
                name: "isAttached",
                table: "cmrMedication");
        }
    }
}
