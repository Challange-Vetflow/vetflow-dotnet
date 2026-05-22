using System.ComponentModel.DataAnnotations;
using VetFlow.Domain.Entities;
using VetFlow.Domain.Enums;

namespace VetFlow.Application.DTOs;

public class PetRequest
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public SpeciesEnum Species { get; set; }

    public string Breed { get; set; } = string.Empty;

    [Required(ErrorMessage = "Data de nascimento é obrigatória")]
    public DateOnly BirthDate { get; set; }

    [Range(0.01, 200.0, ErrorMessage = "Peso deve estar entre 0.01 e 200 kg")]
    public double WeightKg { get; set; }

    [Required]
    public Guid TutorId { get; set; }

    public Pet ToDomain() => new(Name, Species, Breed, BirthDate, WeightKg, TutorId);
}

public record PetResponse(
    Guid Id,
    string Name,
    SpeciesEnum Species,
    string Breed,
    DateOnly BirthDate,
    double WeightKg,
    int AgeInMonths,
    Guid TutorId,
    bool Active)
{
    public static PetResponse FromDomain(Pet p) =>
        new(p.Id, p.Name, p.Species, p.Breed, p.BirthDate, p.WeightKg, p.AgeInMonths, p.TutorId, p.Active);
}