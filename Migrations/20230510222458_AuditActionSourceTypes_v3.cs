using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class AuditActionSourceTypes_v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AuditActionSourceTypes",
                columns: new[] { "Id", "ActionSourceType" },
                values: new object[,]
                {
                    { 19, "AccountLogin" },
                    { 20, "AccountLogout" }
                });

            migrationBuilder.InsertData(
                table: "AuditActionTypes",
                columns: new[] { "Id", "ActionType" },
                values: new object[,]
                {
                    { 9, "Login" },
                    { 10, "Logout" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AuditActionSourceTypes",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "AuditActionSourceTypes",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "AuditActionTypes",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AuditActionTypes",
                keyColumn: "Id",
                keyValue: 10);
        }
    }
}
