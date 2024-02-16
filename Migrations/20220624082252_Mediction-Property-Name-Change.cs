using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class MedictionPropertyNameChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "genericDrugName",
                table: "medicationConsumptions",
                newName: "drugSubGroup");

            migrationBuilder.RenameColumn(
                name: "genericDrugName",
                table: "medication",
                newName: "drugSubGroup");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "drugSubGroup",
                table: "medicationConsumptions",
                newName: "genericDrugName");

            migrationBuilder.RenameColumn(
                name: "drugSubGroup",
                table: "medication",
                newName: "genericDrugName");
        }
    }
}
