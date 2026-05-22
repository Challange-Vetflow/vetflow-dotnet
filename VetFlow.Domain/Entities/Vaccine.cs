using VetFlow.Domain.Common;

namespace VetFlow.Domain.Entities;

/// <summary>
/// Registro de vacinação do pet.
/// </summary>
public class Vaccine : BaseEntity
{
    public Guid PetId { get; private set; }
    public Pet? Pet { get; private set; }

    public string VaccineName { get; private set; } = string.Empty;
    public DateOnly AppliedAt { get; private set; }
    public DateOnly NextDoseAt { get; private set; }
    public string Batch { get; private set; } = string.Empty;

    private Vaccine() { }

    public Vaccine(Guid petId, string vaccineName, DateOnly appliedAt, DateOnly nextDoseAt, string batch)
    {
        if (string.IsNullOrWhiteSpace(vaccineName)) throw new Exception("Nome da vacina é obrigatório.");
        if (nextDoseAt <= appliedAt) throw new Exception("Próxima dose deve ser após a aplicação.");

        PetId = petId;
        VaccineName = vaccineName.Trim();
        AppliedAt = appliedAt;
        NextDoseAt = nextDoseAt;
        Batch = batch ?? string.Empty;
    }

    public void Update(string vaccineName, DateOnly appliedAt, DateOnly nextDoseAt, string batch)
    {
        if (string.IsNullOrWhiteSpace(vaccineName)) throw new Exception("Nome da vacina é obrigatório.");
        if (nextDoseAt <= appliedAt) throw new Exception("Próxima dose deve ser após a aplicação.");

        VaccineName = vaccineName.Trim();
        AppliedAt = appliedAt;
        NextDoseAt = nextDoseAt;
        Batch = batch ?? string.Empty;
    }

    public bool IsExpired => NextDoseAt < DateOnly.FromDateTime(DateTime.Today);
}
