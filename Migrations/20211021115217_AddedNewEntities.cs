using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OntrackDb.Migrations
{
    public partial class AddedNewEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Npi = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pharmacies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressId = table.Column<int>(type: "int", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FaxNumber = table.Column<int>(type: "int", nullable: false),
                    PharmacyManager = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NcpdpNumber = table.Column<int>(type: "int", nullable: false),
                    NpiNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pharmacies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pharmacies_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AddressId = table.Column<int>(type: "int", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dob = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PhoneNumber = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PharmacyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Patients_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Patients_Pharmacies_PharmacyId",
                        column: x => x.PharmacyId,
                        principalTable: "Pharmacies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Medications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RxNumber = table.Column<int>(type: "int", nullable: false),
                    RxDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DrugName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Direction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Supply = table.Column<int>(type: "int", nullable: false),
                    PrescriberName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastFillDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NextFillDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PayDue = table.Column<int>(type: "int", nullable: false),
                    RfNumber = table.Column<int>(type: "int", nullable: false),
                    ConditionTreated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Medications_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medications_PatientId",
                table: "Medications",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_AddressId",
                table: "Patients",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_PharmacyId",
                table: "Patients",
                column: "PharmacyId");

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacies_AddressId",
                table: "Pharmacies",
                column: "AddressId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Doctors");

            migrationBuilder.DropTable(
                name: "Medications");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "Pharmacies");
        }
    }
}
