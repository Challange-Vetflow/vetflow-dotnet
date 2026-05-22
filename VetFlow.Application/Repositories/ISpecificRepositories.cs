using VetFlow.Domain.Entities;

namespace VetFlow.Application.Repositories;

public interface ITutorRepository
{
    IReadOnlyList<Tutor> GetAll();
    Tutor? GetById(Guid id);
    Tutor? GetByEmail(string email);
    Tutor Add(Tutor tutor);
    bool Update(Tutor tutor);
    bool Delete(Guid id);
    bool ExistsById(Guid id);
    bool ExistsByEmail(string email);
}

public interface IPetRepository
{
    IReadOnlyList<Pet> GetAll();
    IReadOnlyList<Pet> GetByTutorId(Guid tutorId);
    Pet? GetById(Guid id);
    Pet Add(Pet pet);
    bool Update(Pet pet);
    bool Delete(Guid id);
    bool ExistsById(Guid id);
}

public interface IClinicRepository : IRepository<Clinic>
{
    bool ExistsByName(string name);
    void Update(Clinic clinic);
}

public interface IAppointmentRepository
{
    IReadOnlyList<Appointment> GetAll();
    IReadOnlyList<Appointment> GetByPetId(Guid petId);
    IReadOnlyList<Appointment> GetByClinicId(Guid clinicId);
    IReadOnlyList<Appointment> GetPending();
    Appointment? GetById(Guid id);
    Appointment Add(Appointment appointment);
    bool Update(Appointment appointment);
    bool Delete(Guid id);
    bool ExistsById(Guid id);
}

public interface IVaccineRepository
{
    IReadOnlyList<Vaccine> GetAll();
    IReadOnlyList<Vaccine> GetByPetId(Guid petId);
    IReadOnlyList<Vaccine> GetExpired();
    Vaccine? GetById(Guid id);
    Vaccine Add(Vaccine vaccine);
    bool Delete(Guid id);
    bool ExistsById(Guid id);
    void Update(Vaccine vaccine);
}

public interface IMedicationRepository
{
    IReadOnlyList<Medication> GetAll();
    IReadOnlyList<Medication> GetByPetId(Guid petId);
    IReadOnlyList<Medication> GetActive();
    Medication? GetById(Guid id);
    Medication Add(Medication medication);
    bool Update(Medication medication);
    bool Delete(Guid id);
    bool ExistsById(Guid id);
}
