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
                name: "CV_TUTORS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CV_TUTORS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CV_CLINICS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CV_CLINICS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CV_PETS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Species = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Breed = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    WeightKg = table.Column<double>(type: "REAL", nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TutorId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CV_PETS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CV_PETS_CV_TUTORS_TutorId",
                        column: x => x.TutorId,
                        principalTable: "CV_TUTORS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CV_APPOINTMENTS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PetId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClinicId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Completed = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CV_APPOINTMENTS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CV_APPOINTMENTS_CV_PETS_PetId",
                        column: x => x.PetId,
                        principalTable: "CV_PETS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CV_APPOINTMENTS_CV_CLINICS_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "CV_CLINICS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CV_VACCINES",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PetId = table.Column<Guid>(type: "TEXT", nullable: false),
                    VaccineName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AppliedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NextDoseAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Batch = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CV_VACCINES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CV_VACCINES_CV_PETS_PetId",
                        column: x => x.PetId,
                        principalTable: "CV_PETS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CV_MEDICATIONS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PetId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Dosage = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Frequency = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false, defaultValue: "ACTIVE"),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CV_MEDICATIONS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CV_MEDICATIONS_CV_PETS_PetId",
                        column: x => x.PetId,
                        principalTable: "CV_PETS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CV_TUTORS_Email",
                table: "CV_TUTORS",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CV_PETS_TutorId",
                table: "CV_PETS",
                column: "TutorId");

            migrationBuilder.CreateIndex(
                name: "IX_CV_APPOINTMENTS_PetId",
                table: "CV_APPOINTMENTS",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_CV_APPOINTMENTS_ClinicId",
                table: "CV_APPOINTMENTS",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_CV_VACCINES_PetId",
                table: "CV_VACCINES",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_CV_MEDICATIONS_PetId",
                table: "CV_MEDICATIONS",
                column: "PetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "CV_MEDICATIONS");
            migrationBuilder.DropTable(name: "CV_VACCINES");
            migrationBuilder.DropTable(name: "CV_APPOINTMENTS");
            migrationBuilder.DropTable(name: "CV_PETS");
            migrationBuilder.DropTable(name: "CV_CLINICS");
            migrationBuilder.DropTable(name: "CV_TUTORS");
        }
    }
}
