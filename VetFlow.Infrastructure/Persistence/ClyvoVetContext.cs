using VetFlow.Domain.Entities;
using VetFlow.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace VetFlow.Infrastructure.Persistence;

/// <summary>
/// Contexto EF Core do VetFlow.
/// </summary>
public class VetFlowContext(DbContextOptions<VetFlowContext> options) : DbContext(options)
{
    public DbSet<Tutor> Tutors { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<Clinic> Clinics { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Vaccine> Vaccines { get; set; }
    public DbSet<Medication> Medications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VetFlowContext).Assembly);
    }
}
