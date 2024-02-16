using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class Add_Refill_Due_Calc_Columns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "cholesterolRefillDue",
                table: "patient",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "diabetesRefillDue",
                table: "patient",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "rasaRefillDue",
                table: "patient",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "medicationId",
                table: "medicationConsumptions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "inUse",
                table: "medication",
                type: "bit",
                nullable: false,
                defaultValueSql: "1");

            migrationBuilder.AddColumn<bool>(
                name: "refillDue",
                table: "medication",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "RefillDueActivationQueue",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    MedicationId = table.Column<int>(type: "int", nullable: false),
                    ActivationDate = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefillDueActivationQueue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefillDueActivationQueue_patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_medicationConsumptions_medicationId",
                table: "medicationConsumptions",
                column: "medicationId");

            migrationBuilder.CreateIndex(
                name: "IX_RefillDueActivationQueue_PatientId",
                table: "RefillDueActivationQueue",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_medicationConsumptions_medication_medicationId",
                table: "medicationConsumptions",
                column: "medicationId",
                principalTable: "medication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_medicationConsumptions_medication_medicationId",
                table: "medicationConsumptions");

            migrationBuilder.DropTable(
                name: "RefillDueActivationQueue");

            migrationBuilder.DropIndex(
                name: "IX_medicationConsumptions_medicationId",
                table: "medicationConsumptions");

            migrationBuilder.DropColumn(
                name: "cholesterolRefillDue",
                table: "patient");

            migrationBuilder.DropColumn(
                name: "diabetesRefillDue",
                table: "patient");

            migrationBuilder.DropColumn(
                name: "rasaRefillDue",
                table: "patient");

            migrationBuilder.DropColumn(
                name: "medicationId",
                table: "medicationConsumptions");

            migrationBuilder.DropColumn(
                name: "inUse",
                table: "medication");

            migrationBuilder.DropColumn(
                name: "refillDue",
                table: "medication");
        }
    }
}
