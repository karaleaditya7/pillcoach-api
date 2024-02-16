using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class Rename_AuditActionType_Login : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AuditActionTypes",
                keyColumn: "Id",
                keyValue: 9,
                column: "ActionType",
                value: "LoginSuccessful");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AuditActionTypes",
                keyColumn: "Id",
                keyValue: 9,
                column: "ActionType",
                value: "Login");
        }
    }
}
