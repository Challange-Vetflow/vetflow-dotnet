using VetFlow.Domain.Common;

namespace VetFlow.Domain.Entities;

/// <summary>
/// Clínica veterinária parceira.
/// </summary>
public class Clinic : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    public List<Appointment> Appointments { get; private set; } = [];

    private Clinic() { }

    public Clinic(string name, string address, string phone, string email)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new Exception("Nome da clínica é obrigatório.");
        Name = name.Trim();
        Address = address.Trim();
        Phone = phone.Trim();
        Email = email.Trim().ToLowerInvariant();
    }

    public void Update(string name, string address, string phone, string email)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new Exception("Nome da clínica é obrigatório.");
        Name = name.Trim();
        Address = address.Trim();
        Phone = phone.Trim();
        Email = email.Trim().ToLowerInvariant();
    }
}
