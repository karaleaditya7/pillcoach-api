using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class UserCompliance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ComplianceId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserCompliances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BackgroundCheck = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "0"),
                    LiabilityInsurance = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "0"),
                    AnnualHIPPATraining = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "0"),
                    HippaTrainingRecordedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AnnualFraudTraining = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "0"),
                    FraudTrainingRecordedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AnnualCyberTraining = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "0"),
                    CyberTrainingRecordedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCompliances", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ComplianceId",
                table: "AspNetUsers",
                column: "ComplianceId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserCompliances_ComplianceId",
                table: "AspNetUsers",
                column: "ComplianceId",
                principalTable: "UserCompliances",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserCompliances_ComplianceId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "UserCompliances");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ComplianceId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ComplianceId",
                table: "AspNetUsers");
        }
    }
}
