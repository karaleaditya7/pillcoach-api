using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class AddedCmrPatientEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CmrPatientId",
                table: "serviceTakeawayInformation",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "cmrPatient",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    imageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    patientVendorRxID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PharmacyId = table.Column<int>(type: "int", nullable: true),
                    AddressId = table.Column<int>(type: "int", nullable: true),
                    ContactId = table.Column<int>(type: "int", nullable: true),
                    noteId = table.Column<int>(type: "int", nullable: true),
                    ImportDataid = table.Column<int>(type: "int", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cmrPatient", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cmrPatient_address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "address",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_cmrPatient_contact_ContactId",
                        column: x => x.ContactId,
                        principalTable: "contact",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_cmrPatient_importData_ImportDataid",
                        column: x => x.ImportDataid,
                        principalTable: "importData",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_cmrPatient_notes_noteId",
                        column: x => x.noteId,
                        principalTable: "notes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_cmrPatient_pharmacy_PharmacyId",
                        column: x => x.PharmacyId,
                        principalTable: "pharmacy",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_serviceTakeawayInformation_CmrPatientId",
                table: "serviceTakeawayInformation",
                column: "CmrPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_cmrPatient_AddressId",
                table: "cmrPatient",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_cmrPatient_ContactId",
                table: "cmrPatient",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_cmrPatient_ImportDataid",
                table: "cmrPatient",
                column: "ImportDataid");

            migrationBuilder.CreateIndex(
                name: "IX_cmrPatient_noteId",
                table: "cmrPatient",
                column: "noteId");

            migrationBuilder.CreateIndex(
                name: "IX_cmrPatient_PharmacyId",
                table: "cmrPatient",
                column: "PharmacyId");

            migrationBuilder.AddForeignKey(
                name: "FK_serviceTakeawayInformation_cmrPatient_CmrPatientId",
                table: "serviceTakeawayInformation",
                column: "CmrPatientId",
                principalTable: "cmrPatient",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_serviceTakeawayInformation_cmrPatient_CmrPatientId",
                table: "serviceTakeawayInformation");

            migrationBuilder.DropTable(
                name: "cmrPatient");

            migrationBuilder.DropIndex(
                name: "IX_serviceTakeawayInformation_CmrPatientId",
                table: "serviceTakeawayInformation");

            migrationBuilder.DropColumn(
                name: "CmrPatientId",
                table: "serviceTakeawayInformation");
        }
    }
}
