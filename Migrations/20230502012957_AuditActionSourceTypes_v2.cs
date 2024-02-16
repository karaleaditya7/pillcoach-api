using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class AuditActionSourceTypes_v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AuditActionSourceTypes",
                columns: new[] { "Id", "ActionSourceType" },
                values: new object[] { 16, "PatientConsent" });

            migrationBuilder.InsertData(
                table: "AuditActionSourceTypes",
                columns: new[] { "Id", "ActionSourceType" },
                values: new object[] { 17, "PatientCMR" });

            migrationBuilder.InsertData(
                table: "AuditActionSourceTypes",
                columns: new[] { "Id", "ActionSourceType" },
                values: new object[] { 18, "PatientMedRec" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AuditActionSourceTypes",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "AuditActionSourceTypes",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "AuditActionSourceTypes",
                keyColumn: "Id",
                keyValue: 18);
        }
    }
}
