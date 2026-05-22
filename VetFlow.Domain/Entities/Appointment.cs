using VetFlow.Domain.Common;
using VetFlow.Domain.Enums;

namespace VetFlow.Domain.Entities;

/// <summary>
/// Consulta/atendimento do pet na clínica.
/// </summary>
public class Appointment : BaseEntity
{
    public Guid PetId { get; private set; }
    public Pet? Pet { get; private set; }

    public Guid ClinicId { get; private set; }
    public Clinic? Clinic { get; private set; }

    public DateTime ScheduledAt { get; private set; }
    public AppointmentTypeEnum Type { get; private set; }
    public string Notes { get; private set; } = string.Empty;
    public bool Completed { get; private set; }

    private Appointment() { }

    public Appointment(Guid petId, Guid clinicId, DateTime scheduledAt, AppointmentTypeEnum type, string notes)
    {
        if (scheduledAt < DateTime.UtcNow.AddMinutes(-5))
            throw new Exception("Data da consulta não pode ser no passado.");

        PetId = petId;
        ClinicId = clinicId;
        ScheduledAt = scheduledAt;
        Type = type;
        Notes = notes ?? string.Empty;
        Completed = false;
    }

    public void MarkCompleted() => Completed = true;

    public void Update(Guid clinicId, DateTime scheduledAt, AppointmentTypeEnum type, string notes)
    {
        if (scheduledAt < DateTime.UtcNow.AddMinutes(-5))
            throw new Exception("Data da consulta não pode ser no passado.");

        ClinicId = clinicId;
        ScheduledAt = scheduledAt;
        Type = type;
        Notes = notes ?? string.Empty;
    }
}
