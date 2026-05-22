using VetFlow.Domain.Common;
using VetFlow.Domain.Enums;

namespace VetFlow.Domain.Entities;

/// <summary>
/// Animal de estimação.
/// </summary>
public class Pet : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public SpeciesEnum Species { get; private set; }
    public string Breed { get; private set; } = string.Empty;
    public DateOnly BirthDate { get; private set; }
    public double WeightKg { get; private set; }

    public Guid TutorId { get; private set; }
    public Tutor? Tutor { get; private set; }

    public List<Appointment> Appointments { get; private set; } = [];
    public List<Medication> Medications { get; private set; } = [];
    public List<Vaccine> Vaccines { get; private set; } = [];

    private Pet() { }

    public Pet(string name, SpeciesEnum species, string breed, DateOnly birthDate, double weightKg, Guid tutorId)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new Exception("Nome do pet é obrigatório.");
        if (weightKg <= 0) throw new Exception("Peso deve ser maior que zero.");
        if (birthDate > DateOnly.FromDateTime(DateTime.Today)) throw new Exception("Data de nascimento inválida.");

        Name = name.Trim();
        Species = species;
        Breed = string.IsNullOrWhiteSpace(breed) ? "Indefinida" : breed.Trim();
        BirthDate = birthDate;
        WeightKg = weightKg;
        TutorId = tutorId;
    }

    public void Update(string name, SpeciesEnum species, string breed, DateOnly birthDate, double weightKg)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new Exception("Nome do pet é obrigatório.");
        if (weightKg <= 0) throw new Exception("Peso deve ser maior que zero.");
        if (birthDate > DateOnly.FromDateTime(DateTime.Today)) throw new Exception("Data de nascimento inválida.");

        Name = name.Trim();
        Species = species;
        Breed = string.IsNullOrWhiteSpace(breed) ? "Indefinida" : breed.Trim();
        BirthDate = birthDate;
        WeightKg = weightKg;
    }

    public int AgeInMonths
    {
        get
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return (today.Year - BirthDate.Year) * 12 + (today.Month - BirthDate.Month);
        }
    }
}
