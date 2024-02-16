using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class ChangeMedicationConsumptionEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "genericDrugName",
                table: "medicationConsumptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ndcNumber",
                table: "medicationConsumptions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "genericDrugName",
                table: "medicationConsumptions");

            migrationBuilder.DropColumn(
                name: "ndcNumber",
                table: "medicationConsumptions");
        }
    }
}
