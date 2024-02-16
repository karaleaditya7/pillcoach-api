using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class AddTakeawayverifyEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.CreateTable(
                name: "TakeawayVerify",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UUID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TakeawayVerify", x => x.Id);
                });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropTable(
                name: "TakeawayVerify");

          
        }
    }
}
