using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class AddedPropertySBDCNameToCmrMedication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "sbdcName",
                table: "cmrMedication",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sbdcName",
                table: "cmrMedication");
        }
    }
}
