using System.ComponentModel.DataAnnotations;
using VetFlow.Domain.Entities;

namespace VetFlow.Application.DTOs;

/// <summary>DTO de requisição para criar/atualizar tutor.</summary>
public record TutorRequest(
    [property: Required(ErrorMessage = "Nome é obrigatório")]
    [property: StringLength(100, MinimumLength = 2)]
    string Name,

    [property: Required(ErrorMessage = "E-mail é obrigatório")]
    [property: EmailAddress(ErrorMessage = "E-mail inválido")]
    string Email,

    [property: Required(ErrorMessage = "Telefone é obrigatório")]
    [property: StringLength(20, MinimumLength = 8)]
    string Phone)
{
    public Tutor ToDomain() => new(Name, Email, Phone);
}

/// <summary>DTO de resposta de tutor.</summary>
public record TutorResponse(Guid Id, string Name, string Email, string Phone, bool Active)
{
    public static TutorResponse FromDomain(Tutor t) =>
        new(t.Id, t.Name, t.Email, t.Phone, t.Active);
}
