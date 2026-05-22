using VetFlow.Application.Repositories;
using VetFlow.Domain.Entities;
using VetFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace VetFlow.Infrastructure.Repositories;

public sealed class TutorRepository(VetFlowContext ctx) : ITutorRepository
{
    public IReadOnlyList<Tutor> GetAll() =>
        ctx.Tutors.AsNoTracking().OrderBy(t => t.Name).ToList();

    public Tutor? GetById(Guid id) => ctx.Tutors.Include(t => t.Pets).FirstOrDefault(t => t.Id == id);

    public Tutor? GetByEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;
        var norm = email.Trim().ToLowerInvariant();
        return ctx.Tutors.FirstOrDefault(t => t.Email == norm);
    }

    public Tutor Add(Tutor tutor) { ctx.Tutors.Add(tutor); ctx.SaveChanges(); return tutor; }

    public bool Update(Tutor tutor) { ctx.Tutors.Update(tutor); return ctx.SaveChanges() > 0; }

    public bool Delete(Guid id)
    {
        var e = ctx.Tutors.Find(id);
        if (e is null) return false;
        ctx.Tutors.Remove(e);
        ctx.SaveChanges();
        return true;
    }

    public bool ExistsById(Guid id) => ctx.Tutors.Count(t => t.Id == id) > 0;
    public bool ExistsByEmail(string email) => ctx.Tutors.Count(t => t.Email == email.Trim().ToLowerInvariant()) > 0;
}

public sealed class PetRepository(VetFlowContext ctx) : IPetRepository
{
    public IReadOnlyList<Pet> GetAll() =>
        ctx.Pets.AsNoTracking().Include(p => p.Tutor).OrderBy(p => p.Name).ToList();

    public IReadOnlyList<Pet> GetByTutorId(Guid tutorId) =>
        ctx.Pets.AsNoTracking().Where(p => p.TutorId == tutorId).OrderBy(p => p.Name).ToList();

    public Pet? GetById(Guid id) =>
        ctx.Pets.Include(p => p.Tutor).Include(p => p.Vaccines).Include(p => p.Medications)
            .Include(p => p.Appointments).FirstOrDefault(p => p.Id == id);

    public Pet Add(Pet pet) { ctx.Pets.Add(pet); ctx.SaveChanges(); return pet; }

    public bool Update(Pet pet) { ctx.Pets.Update(pet); return ctx.SaveChanges() > 0; }

    public bool Delete(Guid id)
    {
        var e = ctx.Pets.Find(id);
        if (e is null) return false;
        ctx.Pets.Remove(e);
        ctx.SaveChanges();
        return true;
    }

    public bool ExistsById(Guid id) => ctx.Pets.Count(p => p.Id == id) > 0;
}

public sealed class ClinicRepository(VetFlowContext ctx) : Repository<Clinic>(ctx), IClinicRepository
{
    private readonly VetFlowContext _ctx = ctx;

    public bool ExistsByName(string name) =>
        _ctx.Clinics.Count(c => c.Name.ToLower() == name.Trim().ToLowerInvariant()) > 0;

    public void Update(Clinic clinic)
    {
        throw new NotImplementedException();
    }
}

public sealed class AppointmentRepository(VetFlowContext ctx) : IAppointmentRepository
{
    public IReadOnlyList<Appointment> GetAll() =>
        ctx.Appointments.AsNoTracking().Include(a => a.Pet).Include(a => a.Clinic)
            .OrderBy(a => a.ScheduledAt).ToList();

    public IReadOnlyList<Appointment> GetByPetId(Guid petId) =>
        ctx.Appointments.AsNoTracking().Where(a => a.PetId == petId).OrderBy(a => a.ScheduledAt).ToList();

    public IReadOnlyList<Appointment> GetByClinicId(Guid clinicId) =>
        ctx.Appointments.AsNoTracking().Where(a => a.ClinicId == clinicId).OrderBy(a => a.ScheduledAt).ToList();

    public IReadOnlyList<Appointment> GetPending() =>
        ctx.Appointments.AsNoTracking().Where(a => !a.Completed && a.ScheduledAt >= DateTime.UtcNow)
            .OrderBy(a => a.ScheduledAt).ToList();

    public Appointment? GetById(Guid id) =>
        ctx.Appointments.Include(a => a.Pet).Include(a => a.Clinic).FirstOrDefault(a => a.Id == id);

    public Appointment Add(Appointment a) { ctx.Appointments.Add(a); ctx.SaveChanges(); return a; }

    public bool Update(Appointment a) { ctx.Appointments.Update(a); return ctx.SaveChanges() > 0; }

    public bool Delete(Guid id)
    {
        var e = ctx.Appointments.Find(id);
        if (e is null) return false;
        ctx.Appointments.Remove(e);
        ctx.SaveChanges();
        return true;
    }

    public bool ExistsById(Guid id) => ctx.Appointments.Count(a => a.Id == id) > 0;
}

public sealed class VaccineRepository(VetFlowContext ctx) : IVaccineRepository
{
    public IReadOnlyList<Vaccine> GetAll() =>
        ctx.Vaccines.AsNoTracking().OrderBy(v => v.AppliedAt).ToList();

    public IReadOnlyList<Vaccine> GetByPetId(Guid petId) =>
        ctx.Vaccines.AsNoTracking().Where(v => v.PetId == petId).OrderBy(v => v.AppliedAt).ToList();

    public IReadOnlyList<Vaccine> GetExpired() =>
        ctx.Vaccines.AsNoTracking()
            .Where(v => v.NextDoseAt < DateOnly.FromDateTime(DateTime.Today)).ToList();

    public Vaccine? GetById(Guid id) => ctx.Vaccines.Find(id);

    public Vaccine Add(Vaccine v) { ctx.Vaccines.Add(v); ctx.SaveChanges(); return v; }

    public bool Delete(Guid id)
    {
        var e = ctx.Vaccines.Find(id);
        if (e is null) return false;
        ctx.Vaccines.Remove(e);
        ctx.SaveChanges();
        return true;
    }

    public bool ExistsById(Guid id) => ctx.Vaccines.Count(v => v.Id == id) > 0;

    public void Update(Vaccine vaccine)
    {
        throw new NotImplementedException();
    }
}

public sealed class MedicationRepository(VetFlowContext ctx) : IMedicationRepository
{
    public IReadOnlyList<Medication> GetAll() =>
        ctx.Medications.AsNoTracking().OrderBy(m => m.Name).ToList();

    public IReadOnlyList<Medication> GetByPetId(Guid petId) =>
        ctx.Medications.AsNoTracking().Where(m => m.PetId == petId).OrderBy(m => m.StartDate).ToList();

    public IReadOnlyList<Medication> GetActive() =>
        ctx.Medications.AsNoTracking()
            .Where(m => m.Status == Domain.Enums.MedicationStatusEnum.Active).ToList();

    public Medication? GetById(Guid id) => ctx.Medications.Find(id);

    public Medication Add(Medication m) { ctx.Medications.Add(m); ctx.SaveChanges(); return m; }

    public bool Update(Medication m) { ctx.Medications.Update(m); return ctx.SaveChanges() > 0; }

    public bool Delete(Guid id)
    {
        var e = ctx.Medications.Find(id);
        if (e is null) return false;
        ctx.Medications.Remove(e);
        ctx.SaveChanges();
        return true;
    }

    public bool ExistsById(Guid id) => ctx.Medications.Count(m => m.Id == id) > 0;
}