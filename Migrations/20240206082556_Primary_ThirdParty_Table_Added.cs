using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class Primary_ThirdParty_Table_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "language",
                table: "patient",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "primaryThirdPartyId",
                table: "patient",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "primaryThirdParty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    organizationMarketingName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bin = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_primaryThirdParty", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_patient_primaryThirdPartyId",
                table: "patient",
                column: "primaryThirdPartyId");

            migrationBuilder.AddForeignKey(
                name: "FK_patient_primaryThirdParty_primaryThirdPartyId",
                table: "patient",
                column: "primaryThirdPartyId",
                principalTable: "primaryThirdParty",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_patient_primaryThirdParty_primaryThirdPartyId",
                table: "patient");

            migrationBuilder.DropTable(
                name: "primaryThirdParty");

            migrationBuilder.DropIndex(
                name: "IX_patient_primaryThirdPartyId",
                table: "patient");

            migrationBuilder.DropColumn(
                name: "language",
                table: "patient");

            migrationBuilder.DropColumn(
                name: "primaryThirdPartyId",
                table: "patient");
        }
    }
}
