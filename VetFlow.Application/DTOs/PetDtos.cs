using System.ComponentModel.DataAnnotations;
using VetFlow.Domain.Entities;
using VetFlow.Domain.Enums;

namespace VetFlow.Application.DTOs;

/// <summary>DTO de requisição para criar pet.</summary>
public record PetRequest(
    [property: Required(ErrorMessage = "Nome é obrigatório")]
    [property: StringLength(100, MinimumLength = 1)]
    string Name,

    [property: Required]
    SpeciesEnum Species,

    string Breed,

    [property: Required(ErrorMessage = "Data de nascimento é obrigatória")]
    DateOnly BirthDate,

    [property: Range(0.01, 200.0, ErrorMessage = "Peso deve estar entre 0.01 e 200 kg")]
    double WeightKg,

    [property: Required]
    Guid TutorId)
{
    public Pet ToDomain() => new(Name, Species, Breed, BirthDate, WeightKg, TutorId);
}

/// <summary>DTO de resposta de pet.</summary>
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
