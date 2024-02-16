using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OntrackDb.Migrations
{
    public partial class ImportdataNotesEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "importDataId",
                table: "pharmacies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "importDataId",
                table: "Patients",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NoteId",
                table: "Patients",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "importDataId",
                table: "Medications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "importDataId",
                table: "Doctors",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ImportDatas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_datetime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportDatas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    text = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacies_ImportDataid",
                table: "pharmacies",
                column: "importDataId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_ImportDataid",
                table: "Patients",
                column: "importDataId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_NoteId",
                table: "Patients",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Medications_ImportDataid",
                table: "Medications",
                column: "importDataId");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_ImportDataid",
                table: "Doctors",
                column: "importDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_ImportDatas_ImportDataid",
                table: "Doctors",
                column: "importDataId",
                principalTable: "ImportDatas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Medications_ImportDatas_ImportDataid",
                table: "Medications",
                column: "importDataId",
                principalTable: "ImportDatas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_ImportDatas_ImportDataid",
                table: "Patients",
                column: "importDataId",
                principalTable: "ImportDatas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Notes_NoteId",
                table: "Patients",
                column: "NoteId",
                principalTable: "Notes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pharmacies_ImportDatas_ImportDataid",
                table: "pharmacies",
                column: "importDataId",
                principalTable: "ImportDatas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_ImportDatas_ImportDataid",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Medications_ImportDatas_ImportDataid",
                table: "Medications");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_ImportDatas_ImportDataid",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Notes_NoteId",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_Pharmacies_ImportDatas_ImportDataid",
                table: "pharmacies");

            migrationBuilder.DropTable(
                name: "ImportDatas");

            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Pharmacies_ImportDataid",
                table: "pharmacies");

            migrationBuilder.DropIndex(
                name: "IX_Patients_ImportDataid",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_NoteId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Medications_ImportDataid",
                table: "Medications");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_ImportDataid",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "ImportDataid",
                table: "pharmacies");

            migrationBuilder.DropColumn(
                name: "ImportDataid",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "NoteId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "ImportDataid",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "ImportDataid",
                table: "Doctors");
        }
    }
}
