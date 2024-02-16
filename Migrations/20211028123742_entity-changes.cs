using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OntrackDb.Migrations
{
    public partial class entitychanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FaxNumber",
                table: "Pharmacies");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Pharmacies");

            migrationBuilder.DropColumn(
                name: "Dob",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "email",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Doctors");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Patients",
                newName: "Status");

            migrationBuilder.AddColumn<int>(
                name: "ContactId",
                table: "Pharmacies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContactId",
                table: "Patients",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContactId",
                table: "Doctors",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrimaryPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondaryPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrimaryEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondaryEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DoB = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DoctorPharmacy",
                columns: table => new
                {
                    DoctorsId = table.Column<int>(type: "int", nullable: false),
                    PharmaciesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorPharmacy", x => new { x.DoctorsId, x.PharmaciesId });
                    table.ForeignKey(
                        name: "FK_DoctorPharmacy_Doctors_DoctorsId",
                        column: x => x.DoctorsId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DoctorPharmacy_Pharmacies_PharmaciesId",
                        column: x => x.PharmaciesId,
                        principalTable: "Pharmacies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacies_ContactId",
                table: "Pharmacies",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_ContactId",
                table: "Patients",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_ContactId",
                table: "Doctors",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorPharmacy_PharmaciesId",
                table: "DoctorPharmacy",
                column: "PharmaciesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Contacts_ContactId",
                table: "Doctors",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Contacts_ContactId",
                table: "Patients",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pharmacies_Contacts_ContactId",
                table: "Pharmacies",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Contacts_ContactId",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Contacts_ContactId",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_Pharmacies_Contacts_ContactId",
                table: "Pharmacies");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "DoctorPharmacy");

            migrationBuilder.DropIndex(
                name: "IX_Pharmacies_ContactId",
                table: "Pharmacies");

            migrationBuilder.DropIndex(
                name: "IX_Patients_ContactId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_ContactId",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "ContactId",
                table: "Pharmacies");

            migrationBuilder.DropColumn(
                name: "ContactId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "ContactId",
                table: "Doctors");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Patients",
                newName: "status");

            migrationBuilder.AddColumn<int>(
                name: "FaxNumber",
                table: "Pharmacies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Pharmacies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Dob",
                table: "Patients",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PhoneNumber",
                table: "Patients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
