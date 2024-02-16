using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class ImportWizard_StagingTable_Remove_Pharmacist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pharmacist",
                table: "ImportFileStagingData");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Pharmacist",
                table: "ImportFileStagingData",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
