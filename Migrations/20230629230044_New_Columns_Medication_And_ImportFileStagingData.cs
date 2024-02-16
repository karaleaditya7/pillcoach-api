using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class New_Columns_Medication_And_ImportFileStagingData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "consumptionEndDate",
                table: "medication",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DrugSbdcName",
                table: "ImportFileStagingData",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "consumptionEndDate",
                table: "medication");

            migrationBuilder.DropColumn(
                name: "DrugSbdcName",
                table: "ImportFileStagingData");
        }
    }
}
