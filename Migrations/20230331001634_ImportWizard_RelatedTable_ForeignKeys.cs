using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class ImportWizard_RelatedTable_ForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "importSourceFileId",
                table: "patient",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "importSourceFileId",
                table: "medicationConsumptions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "importSourceFileId",
                table: "medication",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "importSourceFileId",
                table: "doctor",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "importSourceFileId",
                table: "patient");

            migrationBuilder.DropColumn(
                name: "importSourceFileId",
                table: "medicationConsumptions");

            migrationBuilder.DropColumn(
                name: "importSourceFileId",
                table: "medication");

            migrationBuilder.DropColumn(
                name: "importSourceFileId",
                table: "doctor");
        }
    }
}
