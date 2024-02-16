﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OntrackDb.Context;

namespace OntrackDb.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20211028132738_column-name-to-camelCase")]
    partial class columnnametocamelCase
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "6.0.0-preview.2.21154.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DoctorPharmacy", b =>
                {
                    b.Property<int>("DoctorsId")
                        .HasColumnType("int");

                    b.Property<int>("PharmaciesId")
                        .HasColumnType("int");

                    b.HasKey("DoctorsId", "PharmaciesId");

                    b.HasIndex("PharmaciesId");

                    b.ToTable("DoctorPharmacy");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("OntrackDb.Authentication.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<int?>("AddressId")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2")
                        .HasColumnName("dateOfBirth");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("firstName");

                    b.Property<string>("ImageName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("imageName");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit")
                        .HasColumnName("isDeleted");

                    b.Property<string>("JobPosition")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("jobPosition");

                    b.Property<DateTime>("LastLogin")
                        .HasColumnType("datetime2")
                        .HasColumnName("lastLogin");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("lastName");

                    b.Property<int?>("LicensesId")
                        .HasColumnType("int");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.HasIndex("LicensesId");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("OntrackDb.Entities.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AddressLineOne")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("addressLineOne");

                    b.Property<string>("AddressLineTwo")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("addressLineTwo");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("city");

                    b.Property<string>("State")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("state");

                    b.Property<string>("ZipCode")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("zipCode");

                    b.HasKey("Id");

                    b.ToTable("address");
                });

            modelBuilder.Entity("OntrackDb.Entities.Contact", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DoB")
                        .HasColumnType("datetime2")
                        .HasColumnName("dob");

                    b.Property<string>("Fax")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("fax");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("firstName");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("lastName");

                    b.Property<string>("PrimaryEmail")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("primaryEmail");

                    b.Property<string>("PrimaryPhone")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("primaryPhone");

                    b.Property<string>("SecondaryEmail")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("secondaryEmail");

                    b.Property<string>("SecondaryPhone")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("secondaryPhone");

                    b.HasKey("Id");

                    b.ToTable("contact");
                });

            modelBuilder.Entity("OntrackDb.Entities.Doctor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ContactId")
                        .HasColumnType("int");

                    b.Property<int?>("ImportDataid")
                        .HasColumnType("int");

                    b.Property<int>("Npi")
                        .HasColumnType("int")
                        .HasColumnName("npi");

                    b.HasKey("Id");

                    b.HasIndex("ContactId");

                    b.HasIndex("ImportDataid");

                    b.ToTable("doctor");
                });

            modelBuilder.Entity("OntrackDb.Entities.ImportData", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("created_datetime")
                        .HasColumnType("datetime2");

                    b.Property<string>("data")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("status")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("importData");
                });

            modelBuilder.Entity("OntrackDb.Entities.Licenses", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("expirationDate");

                    b.Property<string>("IssueState")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("issueState");

                    b.Property<string>("Number")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("number");

                    b.HasKey("Id");

                    b.ToTable("licenses");
                });

            modelBuilder.Entity("OntrackDb.Entities.Medication", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ConditionTreated")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("conditionTreated");

                    b.Property<string>("Direction")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("direction");

                    b.Property<string>("DrugName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("drugName");

                    b.Property<int?>("ImportDataid")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastFillDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("lastFillDate");

                    b.Property<DateTime>("NextFillDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("nextFillDate");

                    b.Property<int?>("PatientId")
                        .HasColumnType("int");

                    b.Property<int>("PayDue")
                        .HasColumnType("int")
                        .HasColumnName("payDue");

                    b.Property<string>("PrescriberName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("prescriberName");

                    b.Property<int>("Quantity")
                        .HasColumnType("int")
                        .HasColumnName("quantity");

                    b.Property<int>("RfNumber")
                        .HasColumnType("int")
                        .HasColumnName("rfNumber");

                    b.Property<DateTime>("RxDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("rxDate");

                    b.Property<int>("RxNumber")
                        .HasColumnType("int")
                        .HasColumnName("rxNumber");

                    b.Property<int>("Supply")
                        .HasColumnType("int")
                        .HasColumnName("supply");

                    b.HasKey("Id");

                    b.HasIndex("ImportDataid");

                    b.HasIndex("PatientId");

                    b.ToTable("medication");
                });

            modelBuilder.Entity("OntrackDb.Entities.Note", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("text")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("note");
                });

            modelBuilder.Entity("OntrackDb.Entities.Patient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AddressId")
                        .HasColumnType("int");

                    b.Property<int?>("ContactId")
                        .HasColumnType("int");

                    b.Property<string>("ImageName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("imageName");

                    b.Property<int?>("ImportDataid")
                        .HasColumnType("int");

                    b.Property<int?>("NoteId")
                        .HasColumnType("int");

                    b.Property<int?>("PharmacyId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("status");

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.HasIndex("ContactId");

                    b.HasIndex("ImportDataid");

                    b.HasIndex("NoteId");

                    b.HasIndex("PharmacyId");

                    b.ToTable("patient");
                });

            modelBuilder.Entity("OntrackDb.Entities.Pharmacy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AddressId")
                        .HasColumnType("int");

                    b.Property<int?>("ContactId")
                        .HasColumnType("int");

                    b.Property<string>("ImageName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("imageName");

                    b.Property<int?>("ImportDataid")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastUpdate")
                        .HasColumnType("datetime2")
                        .HasColumnName("lastUpdate");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("name");

                    b.Property<int>("NcpdpNumber")
                        .HasColumnType("int")
                        .HasColumnName("ncpdpNumber");

                    b.Property<int>("NpiNumber")
                        .HasColumnType("int")
                        .HasColumnName("npiNumber");

                    b.Property<string>("PharmacyManager")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("pharmacyManager");

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.HasIndex("ContactId");

                    b.HasIndex("ImportDataid");

                    b.ToTable("pharmacy");
                });

            modelBuilder.Entity("DoctorPharmacy", b =>
                {
                    b.HasOne("OntrackDb.Entities.Doctor", null)
                        .WithMany()
                        .HasForeignKey("DoctorsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OntrackDb.Entities.Pharmacy", null)
                        .WithMany()
                        .HasForeignKey("PharmaciesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("OntrackDb.Authentication.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("OntrackDb.Authentication.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OntrackDb.Authentication.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("OntrackDb.Authentication.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OntrackDb.Authentication.User", b =>
                {
                    b.HasOne("OntrackDb.Entities.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId");

                    b.HasOne("OntrackDb.Entities.Licenses", "Licenses")
                        .WithMany()
                        .HasForeignKey("LicensesId");

                    b.Navigation("Address");

                    b.Navigation("Licenses");
                });

            modelBuilder.Entity("OntrackDb.Entities.Doctor", b =>
                {
                    b.HasOne("OntrackDb.Entities.Contact", "Contact")
                        .WithMany()
                        .HasForeignKey("ContactId");

                    b.HasOne("OntrackDb.Entities.ImportData", "ImportData")
                        .WithMany()
                        .HasForeignKey("ImportDataid");

                    b.Navigation("Contact");

                    b.Navigation("ImportData");
                });

            modelBuilder.Entity("OntrackDb.Entities.Medication", b =>
                {
                    b.HasOne("OntrackDb.Entities.ImportData", "ImportData")
                        .WithMany()
                        .HasForeignKey("ImportDataid");

                    b.HasOne("OntrackDb.Entities.Patient", "Patient")
                        .WithMany("Medications")
                        .HasForeignKey("PatientId");

                    b.Navigation("ImportData");

                    b.Navigation("Patient");
                });

            modelBuilder.Entity("OntrackDb.Entities.Patient", b =>
                {
                    b.HasOne("OntrackDb.Entities.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId");

                    b.HasOne("OntrackDb.Entities.Contact", "Contact")
                        .WithMany()
                        .HasForeignKey("ContactId");

                    b.HasOne("OntrackDb.Entities.ImportData", "ImportData")
                        .WithMany()
                        .HasForeignKey("ImportDataid");

                    b.HasOne("OntrackDb.Entities.Note", "Note")
                        .WithMany()
                        .HasForeignKey("NoteId");

                    b.HasOne("OntrackDb.Entities.Pharmacy", "Pharmacy")
                        .WithMany("Patients")
                        .HasForeignKey("PharmacyId");

                    b.Navigation("Address");

                    b.Navigation("Contact");

                    b.Navigation("ImportData");

                    b.Navigation("Note");

                    b.Navigation("Pharmacy");
                });

            modelBuilder.Entity("OntrackDb.Entities.Pharmacy", b =>
                {
                    b.HasOne("OntrackDb.Entities.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId");

                    b.HasOne("OntrackDb.Entities.Contact", "Contact")
                        .WithMany()
                        .HasForeignKey("ContactId");

                    b.HasOne("OntrackDb.Entities.ImportData", "ImportData")
                        .WithMany()
                        .HasForeignKey("ImportDataid");

                    b.Navigation("Address");

                    b.Navigation("Contact");

                    b.Navigation("ImportData");
                });

            modelBuilder.Entity("OntrackDb.Entities.Patient", b =>
                {
                    b.Navigation("Medications");
                });

            modelBuilder.Entity("OntrackDb.Entities.Pharmacy", b =>
                {
                    b.Navigation("Patients");
                });
#pragma warning restore 612, 618
        }
    }
}
