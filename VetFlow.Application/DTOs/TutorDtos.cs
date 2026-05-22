using System.ComponentModel.DataAnnotations;
using VetFlow.Domain.Entities;

namespace VetFlow.Application.DTOs;

/// <summary>DTO de requisição para criar/atualizar tutor.</summary>
public class TutorRequest
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefone é obrigatório")]
    [StringLength(20, MinimumLength = 8)]
    public string Phone { get; set; } = string.Empty;

    public Tutor ToDomain() => new(Name, Email, Phone);
}

/// <summary>DTO de resposta de tutor.</summary>
public record TutorResponse(Guid Id, string Name, string Email, string Phone, bool Active)
{
    public static TutorResponse FromDomain(Tutor t) =>
        new(t.Id, t.Name, t.Email, t.Phone, t.Active);
}
