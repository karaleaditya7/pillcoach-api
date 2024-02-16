using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class ImportWizard_Columns_RecordCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalImported",
                table: "ImportSourceFiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalRecords",
                table: "ImportSourceFiles",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalImported",
                table: "ImportSourceFiles");

            migrationBuilder.DropColumn(
                name: "TotalRecords",
                table: "ImportSourceFiles");
        }
    }
}
