using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class AddedNewEntityServiceTakeawayInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "serviceTakeawayInformation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cmrCompleted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isVaccination = table.Column<bool>(type: "bit", nullable: false),
                    isDiscussExerciseDiet = table.Column<bool>(type: "bit", nullable: false),
                    vaccineName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cmrSendType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cmrReceiveType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    takeawayTypeInformationType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactId = table.Column<int>(type: "int", nullable: true),
                    AddressId = table.Column<int>(type: "int", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    languageTypeTemplate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    patientTakeawayDeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isPatientCongnitivelyImpaired = table.Column<bool>(type: "bit", nullable: false),
                    isFollowUpAppointment = table.Column<bool>(type: "bit", nullable: false),
                    takeawayReceiveType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isPatientLongTermFacility = table.Column<bool>(type: "bit", nullable: false),
                    additionalNotes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_serviceTakeawayInformation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_serviceTakeawayInformation_address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "address",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_serviceTakeawayInformation_contact_ContactId",
                        column: x => x.ContactId,
                        principalTable: "contact",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_serviceTakeawayInformation_patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patient",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_serviceTakeawayInformation_AddressId",
                table: "serviceTakeawayInformation",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_serviceTakeawayInformation_ContactId",
                table: "serviceTakeawayInformation",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_serviceTakeawayInformation_PatientId",
                table: "serviceTakeawayInformation",
                column: "PatientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "serviceTakeawayInformation");
        }
    }
}
