using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class ReconciliationModuleChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "actionItemReconciliationToDo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_actionItemReconciliationToDo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "medicationReconciliation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rxNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    rxVendorRxID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    rxDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    drugName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    direction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    DoctorPrescribedId = table.Column<int>(type: "int", nullable: true),
                    supply = table.Column<int>(type: "int", nullable: false),
                    prescriberName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lastFillDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    nextFillDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    payDue = table.Column<int>(type: "int", nullable: false),
                    rfNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    conditionTreated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImportDataid = table.Column<int>(type: "int", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    condition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    optionalCondition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    refillsRemaining = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ndcNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    drugSubGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    genericName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isInclude = table.Column<bool>(type: "bit", nullable: false),
                    isAttached = table.Column<bool>(type: "bit", nullable: false),
                    isExclude = table.Column<bool>(type: "bit", nullable: false),
                    sbdcName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medicationReconciliation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_medicationReconciliation_doctor_DoctorPrescribedId",
                        column: x => x.DoctorPrescribedId,
                        principalTable: "doctor",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_medicationReconciliation_importData_ImportDataid",
                        column: x => x.ImportDataid,
                        principalTable: "importData",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_medicationReconciliation_patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patient",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "reconciliationAllergy",
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
                    table.PrimaryKey("PK_reconciliationAllergy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_reconciliationAllergy_medicationSubstance_MedicationSubstanceId",
                        column: x => x.MedicationSubstanceId,
                        principalTable: "medicationSubstance",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_reconciliationAllergy_patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patient",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_reconciliationAllergy_reaction_ReactionId",
                        column: x => x.ReactionId,
                        principalTable: "reaction",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "reconciliationSideeffect",
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
                    table.PrimaryKey("PK_reconciliationSideeffect", x => x.Id);
                    table.ForeignKey(
                        name: "FK_reconciliationSideeffect_medicationSubstance_MedicationSubstanceId",
                        column: x => x.MedicationSubstanceId,
                        principalTable: "medicationSubstance",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_reconciliationSideeffect_patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patient",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_reconciliationSideeffect_reaction_ReactionId",
                        column: x => x.ReactionId,
                        principalTable: "reaction",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "serviceTakeAwayMedReconciliation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    recCompleted = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isVaccination = table.Column<bool>(type: "bit", nullable: true),
                    isDiscussExerciseDiet = table.Column<bool>(type: "bit", nullable: true),
                    vaccineName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    recSendType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    recReceiveType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    recReceiveTypeFirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    recReceiveTypeLastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    takeawayTypeInformationType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactId = table.Column<int>(type: "int", nullable: true),
                    AddressId = table.Column<int>(type: "int", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    languageTypeTemplate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    patientTakeawayDeliveryDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isPatientCongnitivelyImpaired = table.Column<bool>(type: "bit", nullable: true),
                    descriptionCognitivelyImpaired = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isFollowUpAppointment = table.Column<bool>(type: "bit", nullable: true),
                    AppointmentID = table.Column<int>(type: "int", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    takeawayReceiveType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isPatientLongTermFacility = table.Column<bool>(type: "bit", nullable: true),
                    additionalNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CmrPatientId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_serviceTakeAwayMedReconciliation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_serviceTakeAwayMedReconciliation_address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "address",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_serviceTakeAwayMedReconciliation_appointment_AppointmentID",
                        column: x => x.AppointmentID,
                        principalTable: "appointment",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_serviceTakeAwayMedReconciliation_cmrPatient_CmrPatientId",
                        column: x => x.CmrPatientId,
                        principalTable: "cmrPatient",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_serviceTakeAwayMedReconciliation_contact_ContactId",
                        column: x => x.ContactId,
                        principalTable: "contact",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_serviceTakeAwayMedReconciliation_patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patient",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "nonRelatedRecocilationToDo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    ActionItemReconciliationToDoId = table.Column<int>(type: "int", nullable: true),
                    patientToDo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nonRelatedRecocilationToDo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_nonRelatedRecocilationToDo_actionItemReconciliationToDo_ActionItemReconciliationToDoId",
                        column: x => x.ActionItemReconciliationToDoId,
                        principalTable: "actionItemReconciliationToDo",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_nonRelatedRecocilationToDo_patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patient",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "recocilationToDoRelated",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicationReconciliationId = table.Column<int>(type: "int", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    patientToDo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recocilationToDoRelated", x => x.Id);
                    table.ForeignKey(
                        name: "FK_recocilationToDoRelated_medicationReconciliation_MedicationReconciliationId",
                        column: x => x.MedicationReconciliationId,
                        principalTable: "medicationReconciliation",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_recocilationToDoRelated_patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patient",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "vaccineReconciliation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceTakeAwayMedReconciliationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vaccineReconciliation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vaccineReconciliation_serviceTakeAwayMedReconciliation_ServiceTakeAwayMedReconciliationId",
                        column: x => x.ServiceTakeAwayMedReconciliationId,
                        principalTable: "serviceTakeAwayMedReconciliation",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_medicationReconciliation_DoctorPrescribedId",
                table: "medicationReconciliation",
                column: "DoctorPrescribedId");

            migrationBuilder.CreateIndex(
                name: "IX_medicationReconciliation_ImportDataid",
                table: "medicationReconciliation",
                column: "ImportDataid");

            migrationBuilder.CreateIndex(
                name: "IX_medicationReconciliation_PatientId",
                table: "medicationReconciliation",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_nonRelatedRecocilationToDo_ActionItemReconciliationToDoId",
                table: "nonRelatedRecocilationToDo",
                column: "ActionItemReconciliationToDoId");

            migrationBuilder.CreateIndex(
                name: "IX_nonRelatedRecocilationToDo_PatientId",
                table: "nonRelatedRecocilationToDo",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_recocilationToDoRelated_MedicationReconciliationId",
                table: "recocilationToDoRelated",
                column: "MedicationReconciliationId");

            migrationBuilder.CreateIndex(
                name: "IX_recocilationToDoRelated_PatientId",
                table: "recocilationToDoRelated",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_reconciliationAllergy_MedicationSubstanceId",
                table: "reconciliationAllergy",
                column: "MedicationSubstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_reconciliationAllergy_PatientId",
                table: "reconciliationAllergy",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_reconciliationAllergy_ReactionId",
                table: "reconciliationAllergy",
                column: "ReactionId");

            migrationBuilder.CreateIndex(
                name: "IX_reconciliationSideeffect_MedicationSubstanceId",
                table: "reconciliationSideeffect",
                column: "MedicationSubstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_reconciliationSideeffect_PatientId",
                table: "reconciliationSideeffect",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_reconciliationSideeffect_ReactionId",
                table: "reconciliationSideeffect",
                column: "ReactionId");

            migrationBuilder.CreateIndex(
                name: "IX_serviceTakeAwayMedReconciliation_AddressId",
                table: "serviceTakeAwayMedReconciliation",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_serviceTakeAwayMedReconciliation_AppointmentID",
                table: "serviceTakeAwayMedReconciliation",
                column: "AppointmentID");

            migrationBuilder.CreateIndex(
                name: "IX_serviceTakeAwayMedReconciliation_CmrPatientId",
                table: "serviceTakeAwayMedReconciliation",
                column: "CmrPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_serviceTakeAwayMedReconciliation_ContactId",
                table: "serviceTakeAwayMedReconciliation",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_serviceTakeAwayMedReconciliation_PatientId",
                table: "serviceTakeAwayMedReconciliation",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_vaccineReconciliation_ServiceTakeAwayMedReconciliationId",
                table: "vaccineReconciliation",
                column: "ServiceTakeAwayMedReconciliationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "nonRelatedRecocilationToDo");

            migrationBuilder.DropTable(
                name: "recocilationToDoRelated");

            migrationBuilder.DropTable(
                name: "reconciliationAllergy");

            migrationBuilder.DropTable(
                name: "reconciliationSideeffect");

            migrationBuilder.DropTable(
                name: "vaccineReconciliation");

            migrationBuilder.DropTable(
                name: "actionItemReconciliationToDo");

            migrationBuilder.DropTable(
                name: "medicationReconciliation");

            migrationBuilder.DropTable(
                name: "serviceTakeAwayMedReconciliation");
        }
    }
}
