using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class Add_Tables_ImportWizard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImportFileStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportFileStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImportSourceFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Filename = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PharmacyId = table.Column<int>(type: "int", nullable: false),
                    BlobName = table.Column<string>(type: "varchar(50)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UploadDateUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StagingStartTimeUTC = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StagingEndTimeUTC = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ImportStartTimeUTC = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ImportEndTimeUTC = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ImportStatusId = table.Column<int>(type: "int", nullable: false),
                    ColumnMappingsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorStack = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportSourceFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImportSourceFiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImportSourceFiles_ImportFileStatus_ImportStatusId",
                        column: x => x.ImportStatusId,
                        principalTable: "ImportFileStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImportSourceFiles_pharmacy_PharmacyId",
                        column: x => x.PharmacyId,
                        principalTable: "pharmacy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImportFileStagingData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImportSourceFileId = table.Column<int>(type: "int", nullable: false),
                    RowNo = table.Column<int>(type: "int", nullable: false),
                    ErrorsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PharmacyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PharmacyNPI = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Pharmacist = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PatientIdentifier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PatientFirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PatientLastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PatientDateofBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PatientPrimaryAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PatientPrimaryCity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PatientPrimaryState = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PatientPrimaryZipCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PatientPrimaryPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PatientEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PatientGender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PatientLanguage = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PatientRace = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PrescriberFirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PrescriberLastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PrescriberNPI = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PrescriberPrimaryAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PrescriberPrimaryCity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PrescriberPrimaryState = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PrescriberPrimaryZip = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PrescriberPrimaryPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PrescriberFaxNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RxNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RefillNumber = table.Column<int>(type: "int", nullable: true),
                    DateFilled = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DaysSupply = table.Column<int>(type: "int", nullable: true),
                    RefillsRemaining = table.Column<int>(type: "int", nullable: true),
                    DispensedQuantity = table.Column<int>(type: "int", nullable: true),
                    DispensedItemNDC = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DispensedItemName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Directions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientPaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportFileStagingData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImportFileStagingData_ImportSourceFiles_ImportSourceFileId",
                        column: x => x.ImportSourceFileId,
                        principalTable: "ImportSourceFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ImportFileStatus",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Draft" },
                    { 2, "Uploaded" },
                    { 3, "Staging In Progress" },
                    { 4, "Staging Completed" },
                    { 5, "Import In Progress" },
                    { 6, "Imported" },
                    { 7, "Failed" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImportFileStagingData_ImportSourceFileId",
                table: "ImportFileStagingData",
                column: "ImportSourceFileId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportSourceFiles_ImportStatusId",
                table: "ImportSourceFiles",
                column: "ImportStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportSourceFiles_PharmacyId",
                table: "ImportSourceFiles",
                column: "PharmacyId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportSourceFiles_UserId",
                table: "ImportSourceFiles",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImportFileStagingData");

            migrationBuilder.DropTable(
                name: "ImportSourceFiles");

            migrationBuilder.DropTable(
                name: "ImportFileStatus");
        }
    }
}
