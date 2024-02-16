using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class noteentityupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_patient_notes_NoteId",
            //    table: "patient");

            migrationBuilder.DropIndex(
                name: "IX_patient_NoteId",
                table: "patient");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_notes",
            //    table: "note");

            //migrationBuilder.RenameTable(
            //    name: "notes",
            //    newName: "notes");

            migrationBuilder.RenameColumn(
                name: "NoteId",
                table: "patient",
                newName: "noteId");

            migrationBuilder.AddColumn<int>(
                name: "NoteId",
                table: "pharmacy",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "pharmacy",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "patient",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "lastUpdated",
                table: "notes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_notes",
            //    table: "notes",
            //    column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_pharmacy_NoteId",
                table: "pharmacy",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_patient_noteId",
                table: "patient",
                column: "noteId",
                unique: true,
                filter: "[noteId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_patient_notes_noteId",
                table: "patient",
                column: "noteId",
                principalTable: "notes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_pharmacy_notes_NoteId",
                table: "pharmacy",
                column: "NoteId",
                principalTable: "notes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_patient_notes_noteId",
                table: "patient");

            migrationBuilder.DropForeignKey(
                name: "FK_pharmacy_notes_NoteId",
                table: "pharmacy");

            migrationBuilder.DropIndex(
                name: "IX_pharmacy_NoteId",
                table: "pharmacy");

            migrationBuilder.DropIndex(
                name: "IX_patient_noteId",
                table: "patient");

            migrationBuilder.DropPrimaryKey(
                name: "PK_notes",
                table: "notes");

            migrationBuilder.DropColumn(
                name: "NoteId",
                table: "pharmacy");

            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "pharmacy");

            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "patient");

            migrationBuilder.DropColumn(
                name: "lastUpdated",
                table: "notes");

            migrationBuilder.RenameTable(
                name: "notes",
                newName: "note");

            migrationBuilder.RenameColumn(
                name: "noteId",
                table: "patient",
                newName: "NoteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_note",
                table: "note",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_NoteId",
                table: "patient",
                column: "NoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_patient_note_NoteId",
                table: "patient",
                column: "NoteId",
                principalTable: "note",
                principalColumn: "Id");
        }
    }
}
