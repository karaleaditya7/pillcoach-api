using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class Update_ImportFileStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ImportFileStatus",
                keyColumn: "Id",
                keyValue: 7,
                column: "Name",
                value: "Staging Failed");

            migrationBuilder.InsertData(
                table: "ImportFileStatus",
                columns: new[] { "Id", "Name" },
                values: new object[] { 9, "Import Failed" });

            migrationBuilder.InsertData(
                table: "ImportFileStatus",
                columns: new[] { "Id", "Name" },
                values: new object[] { 10, "Rejected" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ImportFileStatus",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ImportFileStatus",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.UpdateData(
                table: "ImportFileStatus",
                keyColumn: "Id",
                keyValue: 7,
                column: "Name",
                value: "Failed");
        }
    }
}
