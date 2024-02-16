using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class New_AuditActionType_LoginFail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AuditActionTypes",
                columns: new[] { "Id", "ActionType" },
                values: new object[] { 11, "LoginFail" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AuditActionTypes",
                keyColumn: "Id",
                keyValue: 11);
        }
    }
}
