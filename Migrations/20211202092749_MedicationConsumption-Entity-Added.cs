using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class MedicationConsumptionEntityAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "medicationConsumptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rxVendorRxID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    rxNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    patientId = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medicationConsumptions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "medicationConsumptions");
        }
    }
}
