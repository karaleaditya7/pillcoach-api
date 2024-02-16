using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class AddedMessageEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "message",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ToUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    messageText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sentDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    readDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_message", x => x.Id);
                    table.ForeignKey(
                        name: "FK_message_AspNetUsers_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_message_AspNetUsers_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_message_FromUserId",
                table: "message",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_message_ToUserId",
                table: "message",
                column: "ToUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "message");
        }
    }
}
