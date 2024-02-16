using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class medicationstagingtableprimarythirdpartyprimarythirdpartybincolumnadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "primaryThirdParty",
                table: "medication",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "primaryThirdPartyBin",
                table: "medication",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryThirdParty",
                table: "ImportFileStagingData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryThirdPartyBin",
                table: "ImportFileStagingData",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "primaryThirdParty",
                table: "medication");

            migrationBuilder.DropColumn(
                name: "primaryThirdPartyBin",
                table: "medication");

            migrationBuilder.DropColumn(
                name: "PrimaryThirdParty",
                table: "ImportFileStagingData");

            migrationBuilder.DropColumn(
                name: "PrimaryThirdPartyBin",
                table: "ImportFileStagingData");
        }
    }
}
