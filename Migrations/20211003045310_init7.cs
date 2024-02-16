using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OntrackDb.Migrations
{
    public partial class init7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LicensesId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Licenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Licenses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LicensesId",
                table: "AspNetUsers",
                column: "LicensesId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Licenses_LicensesId",
                table: "AspNetUsers",
                column: "LicensesId",
                principalTable: "Licenses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Licenses_LicensesId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Licenses");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LicensesId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LicensesId",
                table: "AspNetUsers");
        }
    }
}
