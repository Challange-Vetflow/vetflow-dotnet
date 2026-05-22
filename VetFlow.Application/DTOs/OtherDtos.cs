using System.ComponentModel.DataAnnotations;
using VetFlow.Domain.Entities;
using VetFlow.Domain.Enums;

namespace VetFlow.Application.DTOs;

// ── CLINIC ──────────────────────────────────────────────────────────────────

public class ClinicRequest
{
    [Required][StringLength(200, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Address { get; set; } = string.Empty;

    [Required]
    public string Phone { get; set; } = string.Empty;

    [Required][EmailAddress]
    public string Email { get; set; } = string.Empty;

    public Clinic ToDomain() => new(Name, Address, Phone, Email);
}

public record ClinicResponse(Guid Id, string Name, string Address, string Phone, string Email, bool Active)
{
    public static ClinicResponse FromDomain(Clinic c) =>
        new(c.Id, c.Name, c.Address, c.Phone, c.Email, c.Active);
}

// ── APPOINTMENT ─────────────────────────────────────────────────────────────

public class AppointmentRequest
{
    [Required]
    public Guid PetId { get; set; }

    [Required]
    public Guid ClinicId { get; set; }

    [Required]
    public DateTime ScheduledAt { get; set; }

    [Required]
    public AppointmentTypeEnum Type { get; set; }

    public string Notes { get; set; } = string.Empty;

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

public class VaccineRequest
{
    [Required]
    public Guid PetId { get; set; }

    [Required][StringLength(100, MinimumLength = 2)]
    public string VaccineName { get; set; } = string.Empty;

    [Required]
    public DateOnly AppliedAt { get; set; }

    [Required]
    public DateOnly NextDoseAt { get; set; }

    public string Batch { get; set; } = string.Empty;

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

public class MedicationRequest
{
    [Required]
    public Guid PetId { get; set; }

    [Required][StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Dosage { get; set; } = string.Empty;

    [Required]
    public string Frequency { get; set; } = string.Empty;

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }

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