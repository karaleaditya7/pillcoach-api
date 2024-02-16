using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class AddedNewEntitySafetyDisposal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "safetyDisposal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ADDLCOINFO = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ADDRESS1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ADDRESS2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CITY = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    STATE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZIP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LATITUDE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LONGITUDE = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_safetyDisposal", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "safetyDisposal");
        }
    }
}
