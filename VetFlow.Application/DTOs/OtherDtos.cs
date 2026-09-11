using System.ComponentModel.DataAnnotations;
using VetFlow.Domain.Entities;
using VetFlow.Domain.Enums;

namespace VetFlow.Application.DTOs;

// ── CLINIC ──────────────────────────────────────────────────────────────────

public record ClinicRequest(
    [Required] [StringLength(200, MinimumLength = 2)] string Name,
    [Required] string Address,
    [Required] string Phone,
    [Required] [EmailAddress] string Email)
{
    public Clinic ToDomain() => new(Name, Address, Phone, Email);
}

public record ClinicResponse(Guid Id, string Name, string Address, string Phone, string Email, bool Active)
{
    public static ClinicResponse FromDomain(Clinic c) =>
        new(c.Id, c.Name, c.Address, c.Phone, c.Email, c.Active);
}

// ── APPOINTMENT ─────────────────────────────────────────────────────────────

public record AppointmentRequest(
    [Required] Guid PetId,
    [Required] Guid ClinicId,
    [Required] DateTime ScheduledAt,
    [Required] AppointmentTypeEnum Type,
    string Notes)
{
    public Appointment ToDomain() => new(PetId, ClinicId, ScheduledAt, Type, Notes);
}

public record AppointmentResponse(
    Guid Id,
    Guid PetId,
    Guid ClinicId,
    DateTime ScheduledAt,
    AppointmentTypeEnum Type,
    string Notes,
    bool Completed)
{
    public static AppointmentResponse FromDomain(Appointment a) =>
        new(a.Id, a.PetId, a.ClinicId, a.ScheduledAt, a.Type, a.Notes, a.Completed);
}

// ── VACCINE ──────────────────────────────────────────────────────────────────

public record VaccineRequest(
    [Required] Guid PetId,
    [Required] [StringLength(100, MinimumLength = 2)] string VaccineName,
    [Required] DateOnly AppliedAt,
    [Required] DateOnly NextDoseAt,
    string Batch)
{
    public Vaccine ToDomain() => new(PetId, VaccineName, AppliedAt, NextDoseAt, Batch);
}

public record VaccineResponse(
    Guid Id,
    Guid PetId,
    string VaccineName,
    DateOnly AppliedAt,
    DateOnly NextDoseAt,
    string Batch,
    bool IsExpired)
{
    public static VaccineResponse FromDomain(Vaccine v) =>
        new(v.Id, v.PetId, v.VaccineName, v.AppliedAt, v.NextDoseAt, v.Batch, v.IsExpired);
}

// ── MEDICATION ───────────────────────────────────────────────────────────────

public record MedicationRequest(
    [Required] Guid PetId,
    [Required] [StringLength(100, MinimumLength = 2)] string Name,
    [Required] string Dosage,
    [Required] string Frequency,
    [Required] DateOnly StartDate,
    [Required] DateOnly EndDate)
{
    public Medication ToDomain() => new(PetId, Name, Dosage, Frequency, StartDate, EndDate);
}

public record MedicationResponse(
    Guid Id,
    Guid PetId,
    string Name,
    string Dosage,
    string Frequency,
    DateOnly StartDate,
    DateOnly EndDate,
    MedicationStatusEnum Status)
{
    public static MedicationResponse FromDomain(Medication m) =>
        new(m.Id, m.PetId, m.Name, m.Dosage, m.Frequency, m.StartDate, m.EndDate, m.Status);
}
