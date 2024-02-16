using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class PatientMailListEntityAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isCmrType",
                table: "cmrPatient",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isMedRecType",
                table: "cmrPatient",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "patientMailList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    patientName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    patientId = table.Column<int>(type: "int", nullable: false),
                    pharmacyId = table.Column<int>(type: "int", nullable: false),
                    AddressId = table.Column<int>(type: "int", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    createdDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    deletedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createdUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    modifiedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pharmacyName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patientMailList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_patientMailList_address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "address",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_patientMailList_AddressId",
                table: "patientMailList",
                column: "AddressId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "patientMailList");

            migrationBuilder.DropColumn(
                name: "isCmrType",
                table: "cmrPatient");

            migrationBuilder.DropColumn(
                name: "isMedRecType",
                table: "cmrPatient");
        }
    }
}
