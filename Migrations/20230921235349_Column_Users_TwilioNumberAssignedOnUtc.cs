using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class Column_Users_TwilioNumberAssignedOnUtc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "twilioNumberAssignedOnUtc",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "twilioNumberAssignedOnUtc",
                table: "AspNetUsers");
        }
    }
}
