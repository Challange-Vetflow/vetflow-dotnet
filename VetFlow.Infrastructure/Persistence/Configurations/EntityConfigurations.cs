using VetFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VetFlow.Infrastructure.Persistence.Configurations;

public class TutorConfiguration : IEntityTypeConfiguration<Tutor>
{
    public void Configure(EntityTypeBuilder<Tutor> builder)
    {
        builder.ToTable("CV_Tutors");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).HasMaxLength(100).IsRequired();
        builder.Property(t => t.Email).HasMaxLength(255).IsRequired();
        builder.HasIndex(t => t.Email).IsUnique();
        builder.Property(t => t.Phone).HasMaxLength(20).IsRequired();
        builder.HasMany(t => t.Pets)
            .WithOne(p => p.Tutor)
            .HasForeignKey(p => p.TutorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("CV_Pets");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Breed).HasMaxLength(100);
        builder.Property(p => p.Species).IsRequired();
        builder.Property(p => p.WeightKg).IsRequired();
        builder.Property(p => p.BirthDate)
            .HasConversion(v => v.ToDateTime(TimeOnly.MinValue), v => DateOnly.FromDateTime(v));
        builder.HasMany(p => p.Appointments)
            .WithOne(a => a.Pet)
            .HasForeignKey(a => a.PetId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.Vaccines)
            .WithOne(v => v.Pet)
            .HasForeignKey(v => v.PetId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.Medications)
            .WithOne(m => m.Pet)
            .HasForeignKey(m => m.PetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ClinicConfiguration : IEntityTypeConfiguration<Clinic>
{
    public void Configure(EntityTypeBuilder<Clinic> builder)
    {
        builder.ToTable("CV_Clinics");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Address).HasMaxLength(500);
        builder.Property(c => c.Phone).HasMaxLength(20);
        builder.Property(c => c.Email).HasMaxLength(255);
    }
}

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("CV_Appointments");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.ScheduledAt).IsRequired();
        builder.Property(a => a.Type).IsRequired();
        builder.Property(a => a.Notes).HasMaxLength(1000);
        builder.Property(a => a.Completed).HasDefaultValue(false);
    }
}

public class VaccineConfiguration : IEntityTypeConfiguration<Vaccine>
{
    public void Configure(EntityTypeBuilder<Vaccine> builder)
    {
        builder.ToTable("CV_Vaccines");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.VaccineName).HasMaxLength(100).IsRequired();
        builder.Property(v => v.Batch).HasMaxLength(50);
        builder.Property(v => v.AppliedAt)
            .HasConversion(d => d.ToDateTime(TimeOnly.MinValue), d => DateOnly.FromDateTime(d));
        builder.Property(v => v.NextDoseAt)
            .HasConversion(d => d.ToDateTime(TimeOnly.MinValue), d => DateOnly.FromDateTime(d));
    }
}

public class MedicationConfiguration : IEntityTypeConfiguration<Medication>
{
    public void Configure(EntityTypeBuilder<Medication> builder)
    {
        builder.ToTable("CV_Medications");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Name).HasMaxLength(100).IsRequired();
        builder.Property(m => m.Dosage).HasMaxLength(100);
        builder.Property(m => m.Frequency).HasMaxLength(100);
        builder.Property(m => m.Status).IsRequired();
        builder.Property(m => m.StartDate)
            .HasConversion(d => d.ToDateTime(TimeOnly.MinValue), d => DateOnly.FromDateTime(d));
        builder.Property(m => m.EndDate)
            .HasConversion(d => d.ToDateTime(TimeOnly.MinValue), d => DateOnly.FromDateTime(d));
    }
}
