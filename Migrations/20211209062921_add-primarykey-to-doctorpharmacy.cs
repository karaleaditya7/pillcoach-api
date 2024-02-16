using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class addprimarykeytodoctorpharmacy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "doctorPharmacy",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_doctorPharmacy",
                table: "doctorPharmacy",
                column: "Id");

      
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
          
            migrationBuilder.DropPrimaryKey(
                name: "PK_doctorPharmacy",
                table: "doctorPharmacy");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "doctorPharmacy");
        }
    }
}
