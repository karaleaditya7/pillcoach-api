using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class isincludeEntityadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsExclude",
                table: "Pdc_Medications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isExclude",
                table: "medication",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isInclude",
                table: "medication",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExclude",
                table: "Pdc_Medications");

            migrationBuilder.DropColumn(
                name: "isExclude",
                table: "medication");

            migrationBuilder.DropColumn(
                name: "isInclude",
                table: "medication");
        }
    }
}
