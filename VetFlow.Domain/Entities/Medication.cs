using VetFlow.Domain.Common;
using VetFlow.Domain.Enums;

namespace VetFlow.Domain.Entities;

/// <summary>
/// Medicamento prescrito ao pet.
/// </summary>
public class Medication : BaseEntity
{
    public Guid PetId { get; private set; }
    public Pet? Pet { get; private set; }

    public string Name { get; private set; } = string.Empty;
    public string Dosage { get; private set; } = string.Empty;
    public string Frequency { get; private set; } = string.Empty;
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public MedicationStatusEnum Status { get; private set; }

    private Medication() { }

    public Medication(Guid petId, string name, string dosage, string frequency, DateOnly startDate, DateOnly endDate)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new Exception("Nome do medicamento é obrigatório.");
        if (endDate <= startDate) throw new Exception("Data de fim deve ser após a data de início.");

        PetId = petId;
        Name = name.Trim();
        Dosage = dosage.Trim();
        Frequency = frequency.Trim();
        StartDate = startDate;
        EndDate = endDate;
        Status = MedicationStatusEnum.Active;
    }

    public void Suspend() => Status = MedicationStatusEnum.Suspended;
    public void Complete() => Status = MedicationStatusEnum.Completed;

    public void Update(string name, string dosage, string frequency, DateOnly startDate, DateOnly endDate)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new Exception("Nome do medicamento é obrigatório.");
        if (endDate <= startDate) throw new Exception("Data de fim deve ser após a data de início.");

        Name = name.Trim();
        Dosage = dosage.Trim();
        Frequency = frequency.Trim();
        StartDate = startDate;
        EndDate = endDate;
    }
}
