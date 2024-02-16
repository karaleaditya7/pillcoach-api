using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class RxNavMedicationIndexAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "sbdcName",
                table: "rxNavMedication",
                type: "nvarchar(500)",
                unicode: false,
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "genericName",
                table: "rxNavMedication",
                type: "nvarchar(500)",
                unicode: false,
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GenericName",
                table: "rxNavMedication",
                column: "genericName");

            migrationBuilder.CreateIndex(
                name: "IX_SBDCName",
                table: "rxNavMedication",
                column: "sbdcName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GenericName",
                table: "rxNavMedication");

            migrationBuilder.DropIndex(
                name: "IX_SBDCName",
                table: "rxNavMedication");

            migrationBuilder.AlterColumn<string>(
                name: "sbdcName",
                table: "rxNavMedication",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldUnicode: false,
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "genericName",
                table: "rxNavMedication",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldUnicode: false,
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
