using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class Add_Table_patientPDC : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "patientPDC",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    PdcMonth = table.Column<DateTime>(type: "date", nullable: false),
                    DurationMonths = table.Column<int>(type: "int", nullable: false),
                    Condition = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    PDC = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: false),
                    TotalDays = table.Column<int>(type: "int", nullable: false),
                    TotalFills = table.Column<int>(type: "int", nullable: false),
                    HasExclusions = table.Column<bool>(type: "bit", nullable: false),
                    Consumptions = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patientPDC", x => x.Id);
                    table.ForeignKey(
                        name: "FK_patientPDC_patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_patientPDC_PatientId_PdcMonth_DurationMonths_Condition",
                table: "patientPDC",
                columns: new[] { "PatientId", "PdcMonth", "DurationMonths", "Condition" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "patientPDC");
        }
    }
}
