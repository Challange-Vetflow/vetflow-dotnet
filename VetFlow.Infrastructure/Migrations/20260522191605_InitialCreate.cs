using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VetFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CV_Clinics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    Phone = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    Active = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CV_Clinics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CV_Tutors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    Active = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CV_Tutors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CV_Pets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Species = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Breed = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    WeightKg = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    TutorId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Active = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CV_Pets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CV_Pets_CV_Tutors_TutorId",
                        column: x => x.TutorId,
                        principalTable: "CV_Tutors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CV_Appointments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PetId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ClinicId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    Type = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Notes = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    Completed = table.Column<bool>(type: "NUMBER(1)", nullable: false, defaultValueSql: "0"),
                    Active = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CV_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CV_Appointments_CV_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "CV_Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CV_Appointments_CV_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "CV_Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CV_Medications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PetId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Dosage = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Frequency = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    StartDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    Status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Active = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CV_Medications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CV_Medications_CV_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "CV_Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CV_Vaccines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PetId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    VaccineName = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    AppliedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    NextDoseAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    Batch = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Active = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CV_Vaccines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CV_Vaccines_CV_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "CV_Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CV_Appointments_ClinicId",
                table: "CV_Appointments",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_CV_Appointments_PetId",
                table: "CV_Appointments",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_CV_Medications_PetId",
                table: "CV_Medications",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_CV_Pets_TutorId",
                table: "CV_Pets",
                column: "TutorId");

            migrationBuilder.CreateIndex(
                name: "IX_CV_Tutors_Email",
                table: "CV_Tutors",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CV_Vaccines_PetId",
                table: "CV_Vaccines",
                column: "PetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CV_Appointments");

            migrationBuilder.DropTable(
                name: "CV_Medications");

            migrationBuilder.DropTable(
                name: "CV_Vaccines");

            migrationBuilder.DropTable(
                name: "CV_Clinics");

            migrationBuilder.DropTable(
                name: "CV_Pets");

            migrationBuilder.DropTable(
                name: "CV_Tutors");
        }
    }
}
