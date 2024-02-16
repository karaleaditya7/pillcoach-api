using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class AddedAllergySideEffectEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "medicationSubstance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medicationSubstance", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "reaction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reaction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "allergy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    MedicationSubstanceId = table.Column<int>(type: "int", nullable: true),
                    ReactionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_allergy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_allergy_medicationSubstance_MedicationSubstanceId",
                        column: x => x.MedicationSubstanceId,
                        principalTable: "medicationSubstance",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_allergy_patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patient",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_allergy_reaction_ReactionId",
                        column: x => x.ReactionId,
                        principalTable: "reaction",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "sideEffect",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    MedicationSubstanceId = table.Column<int>(type: "int", nullable: true),
                    ReactionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sideEffect", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sideEffect_medicationSubstance_MedicationSubstanceId",
                        column: x => x.MedicationSubstanceId,
                        principalTable: "medicationSubstance",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_sideEffect_patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patient",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_sideEffect_reaction_ReactionId",
                        column: x => x.ReactionId,
                        principalTable: "reaction",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_allergy_MedicationSubstanceId",
                table: "allergy",
                column: "MedicationSubstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_allergy_PatientId",
                table: "allergy",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_allergy_ReactionId",
                table: "allergy",
                column: "ReactionId");

            migrationBuilder.CreateIndex(
                name: "IX_sideEffect_MedicationSubstanceId",
                table: "sideEffect",
                column: "MedicationSubstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_sideEffect_PatientId",
                table: "sideEffect",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_sideEffect_ReactionId",
                table: "sideEffect",
                column: "ReactionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "allergy");

            migrationBuilder.DropTable(
                name: "sideEffect");

            migrationBuilder.DropTable(
                name: "medicationSubstance");

            migrationBuilder.DropTable(
                name: "reaction");
        }
    }
}
