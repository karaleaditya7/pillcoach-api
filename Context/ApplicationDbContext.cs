using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OntrackDb.Authentication;
using OntrackDb.Entities;
using OntrackDb.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Context
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<Licenses> Licenses { get; set; }
        public DbSet<Address> Address {  get; set; }    
        public DbSet<Pharmacy> Pharmacies { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ImportData> ImportDatas { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<PharmacyUser> PharmacyUsers { get; set; }
        public DbSet<DoctorPharmacy> doctorPharmacy { get; set; }
        public DbSet<MedicationConsumption> medicationConsumptions { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<AdminNotification> AdminNotifications { get; set; }

        public DbSet<WebSocket> WebSockets { get; set; }
        public DbSet<Pdc_Medication> Pdc_Medications { get; set; }
        public DbSet<CmrMedication> CmrMedications { get; set; }
        public DbSet<RxNavMedication> RxNavMedications { get; set; }
        public DbSet<Allergy> Allergies { get; set; }
        public DbSet<SideEffect> SideEffects { get; set; }
        public DbSet<MedicationSubstance> MedicationSubstances { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<MedicationToDoRelated> MedicationToDoRelateds { get; set; }
        public DbSet<NonRelatedMedicationToDo> NonRelatedMedicationToDos { get; set; }
        public DbSet<ActionItemToDo> ActionItemToDos { get; set; }
        public DbSet<ServiceTakeawayInformation> ServiceTakeawayInformations { get; set; }
        public DbSet<SafetyDisposal> SafetyDisposals { get; set; }

        public DbSet<CmrVaccine> CmrVaccines { get; set; }
        public DbSet<CmrPatient> CmrPatients { get; set; }
        public DbSet<TakeawayVerify> TakeawayVerify { get; set; }

        public DbSet<AuditActionType> AuditActionTypes { get; set; }
        public DbSet<AuditActionSourceType> AuditActionSourceTypes { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<PatientPDC> PatientPDCs { get; set; }

        public DbSet<OtcMedication> OtcMedications { get; set; }
        public DbSet<ReconciliationAllergy> ReconciliationAllergies { get; set; }
        public DbSet<ReconciliationSideeffect> ReconciliationSideeffects { get; set; }
        public DbSet<MedicationReconciliation> MedicationReconciliations { get; set; }
        public DbSet<ServiceTakeAwayMedReconciliation> ServiceTakeAwayMedReconciliations { get; set; }
        public DbSet<VaccineReconciliation> VaccineReconciliations { get; set; }
        public DbSet<NonRelatedRecocilationToDo> NonRelatedRecocilationToDos { get; set; }
        public DbSet<ReconciliationToDoRelated> ReconciliationToDoRelateds { get; set; }
        public DbSet<ActionItemReconciliationToDo> ActionItemReconciliationToDos { get; set; }
        public DbSet<DoctorMedication> DoctorMedications { get; set; }

        public DbSet<ImportSourceFile> ImportSourceFiles { get; set; }
        public DbSet<PatientMailList> PatientMailLists { get; set; }
        public DbSet<PatientCallInfo> PatientCallInfo { get; set; }
        public DbSet<MedicationCondition> MedicationConditions { get; set; }
        public DbSet<ImportFileStagingData> ImportFileStagingData { get; set; }
        public DbSet<PrimaryThirdParty> PrimaryThirdParties { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //builder.Entity<DoctorPharmacy>(builder =>
            //{
            //    builder.HasNoKey();
            //    builder.ToTable("doctorPharmacy");
            //});
            builder.Entity<PharmacyUser>()
                .HasKey(pu => new { pu.UserId, pu.PharmacyId });

            builder.Entity<PharmacyUser>()
                .HasOne<User>(pu => pu.User)
                .WithMany(u => u.PharmacyUsers)
                .HasForeignKey(pu => pu.UserId).OnDelete(DeleteBehavior.Cascade); 

            builder.Entity<PharmacyUser>()
                .HasOne<Pharmacy>(pu => pu.Pharmacy)
                .WithMany(p => p.PharmacyUsers)
                .HasForeignKey(pu => pu.PharmacyId).OnDelete(DeleteBehavior.Cascade); 

            builder.Entity<Note>()
                .HasOne<Patient>()
                .WithOne()
                .HasForeignKey<Patient>("NoteId");


            builder.Entity<Patient>()
                .HasOne(p => p.Note)
                .WithOne(n => n.Patient)
                .HasForeignKey<Patient>(p => p.NoteId);

            builder.ApplyConfigurationsFromAssembly(typeof(AuditLog).Assembly);
        }
    }
}
