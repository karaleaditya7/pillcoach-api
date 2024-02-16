using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class _2FA_Updates_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "lastDeviceId",
                table: "AspNetUsers",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "lastVerifiedDateUTC",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "verificationCode",
                table: "AspNetUsers",
                type: "varchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "codeExpiryDateUTC",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "codeExpiryDateUTC",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "lastDeviceId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "lastVerifiedDateUTC",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "verificationCode",
                table: "AspNetUsers");
        }
    }
}
