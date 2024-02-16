using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class ImportWizard_NewColumnAndStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProcessed",
                table: "ImportFileStagingData",
                type: "bit",
                nullable: false,
                defaultValueSql: "0");

            migrationBuilder.InsertData(
                table: "ImportFileStatus",
                columns: new[] { "Id", "Name" },
                values: new object[] { 8, "Partially Imported" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ImportFileStatus",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DropColumn(
                name: "IsProcessed",
                table: "ImportFileStagingData");
        }
    }
}
