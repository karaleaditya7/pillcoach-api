using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class Notes_Add_UserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "notes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_notes_userId",
                table: "notes",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_notes_AspNetUsers_userId",
                table: "notes",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notes_AspNetUsers_userId",
                table: "notes");

            migrationBuilder.DropIndex(
                name: "IX_notes_userId",
                table: "notes");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "notes");
        }
    }
}
