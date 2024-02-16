using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class ServiceTakeAwayMedRecPropertyAddedTakeawayVerify : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TakeawayVerify",
                table: "TakeawayVerify");

            migrationBuilder.RenameTable(
                name: "TakeawayVerify",
                newName: "takeawayVerify");

            migrationBuilder.RenameColumn(
                name: "UUID",
                table: "takeawayVerify",
                newName: "uUID");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "takeawayVerify",
                newName: "patientId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "takeawayVerify",
                newName: "createdAt");

            migrationBuilder.AddColumn<bool>(
                name: "isServiceTakeAwayInfo",
                table: "takeawayVerify",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isServiceTakeAwayMedRec",
                table: "takeawayVerify",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_takeawayVerify",
                table: "takeawayVerify",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_takeawayVerify",
                table: "takeawayVerify");

            migrationBuilder.DropColumn(
                name: "isServiceTakeAwayInfo",
                table: "takeawayVerify");

            migrationBuilder.DropColumn(
                name: "isServiceTakeAwayMedRec",
                table: "takeawayVerify");

            migrationBuilder.RenameTable(
                name: "takeawayVerify",
                newName: "TakeawayVerify");

            migrationBuilder.RenameColumn(
                name: "uUID",
                table: "TakeawayVerify",
                newName: "UUID");

            migrationBuilder.RenameColumn(
                name: "patientId",
                table: "TakeawayVerify",
                newName: "PatientId");

            migrationBuilder.RenameColumn(
                name: "createdAt",
                table: "TakeawayVerify",
                newName: "CreatedAt");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TakeawayVerify",
                table: "TakeawayVerify",
                column: "Id");
        }
    }
}
