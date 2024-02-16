using Microsoft.EntityFrameworkCore.Migrations;

namespace OntrackDb.Migrations
{
    public partial class vendorRxIdfieldsadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "pharmacyVendorRxId",
                table: "pharmacy",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "patientVendorRxID",
                table: "patient",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "rxPioneerRxID",
                table: "medication",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "prescriberVendorRxID",
                table: "doctor",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "pharmacyVendorRxId",
                table: "pharmacy",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "patientVendorRxID",
                table: "patient",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.RenameColumn(
                name: "rxPioneerRxID",
                table: "medication",
                newName: "rxVendorRxID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pharmacyVendorRxId",
                table: "pharmacy");

            migrationBuilder.DropColumn(
                name: "PatientVendorRxID",
                table: "patient");

            migrationBuilder.DropColumn(
                name: "rxVendorRxID",
                table: "medication");

            migrationBuilder.DropColumn(
                name: "prescriberVendorRxID",
                table: "doctor");
        }
    }
}
