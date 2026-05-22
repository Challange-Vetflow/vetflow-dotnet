using VetFlow.Domain.Common;

namespace VetFlow.Domain.Entities;

/// <summary>
/// Responsável pelo pet (tutor).
/// </summary>
public class Tutor : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;

    public List<Pet> Pets { get; private set; } = [];

    private Tutor() { }

    public Tutor(string name, string email, string phone)
    {
        UpdateName(name);
        UpdateEmail(email);
        UpdatePhone(phone);
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new Exception("Nome é obrigatório.");
        Name = name.Trim();
    }

    public void UpdateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new Exception("E-mail inválido.");
        Email = email.Trim().ToLowerInvariant();
    }

    public void UpdatePhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new Exception("Telefone é obrigatório.");
        Phone = phone.Trim();
    }
}
