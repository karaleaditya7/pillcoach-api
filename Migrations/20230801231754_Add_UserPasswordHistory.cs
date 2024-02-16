using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class Add_UserPasswordHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "passwordSetDateUTC",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserPasswordHistory",
                columns: table => new
                {
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HashPassword = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedDateUTC = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPasswordHistory", x => new { x.UserID, x.HashPassword });
                    table.ForeignKey(
                        name: "FK_UserPasswordHistory_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPasswordHistory");

            migrationBuilder.DropColumn(
                name: "passwordSetDateUTC",
                table: "AspNetUsers");
        }
    }
}
