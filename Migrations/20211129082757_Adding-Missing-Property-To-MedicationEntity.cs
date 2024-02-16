using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class AddingMissingPropertyToMedicationEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_pharmacy_notes_NoteId",
            //    table: "pharmacy");

            //migrationBuilder.DropIndex(
            //    name: "IX_pharmacy_NoteId",
            //    table: "pharmacy");

            //migrationBuilder.RenameColumn(
            //    name: "NoteId",
            //    table: "pharmacy",
            //    newName: "noteId");

            migrationBuilder.AddColumn<string>(
                name: "ndcNumber",
                table: "medication",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "refillsRemaining",
                table: "medication",
                type: "nvarchar(max)",
                nullable: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_pharmacy_noteId",
            //    table: "pharmacy",
            //    column: "noteId",
            //    unique: true,
            //    filter: "[noteId] IS NOT NULL");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_pharmacy_notes_noteId",
            //    table: "pharmacy",
            //    column: "noteId",
            //    principalTable: "notes",
            //    principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_pharmacy_notes_noteId",
            //    table: "pharmacy");

            //migrationBuilder.DropIndex(
            //    name: "IX_pharmacy_noteId",
            //    table: "pharmacy");

            migrationBuilder.DropColumn(
                name: "ndcNumber",
                table: "medication");

            migrationBuilder.DropColumn(
                name: "refillsRemaining",
                table: "medication");

            //migrationBuilder.RenameColumn(
            //    name: "noteId",
            //    table: "pharmacy",
            //    newName: "NoteId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_pharmacy_NoteId",
            //    table: "pharmacy",
            //    column: "NoteId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_pharmacy_notes_NoteId",
            //    table: "pharmacy",
            //    column: "NoteId",
            //    principalTable: "notes",
            //    principalColumn: "Id");
        }
    }
}
