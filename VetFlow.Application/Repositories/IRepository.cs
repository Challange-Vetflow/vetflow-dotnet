using VetFlow.Domain.Common;

namespace VetFlow.Application.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    IReadOnlyList<T> GetAll();
    T? GetById(Guid id);
    T Add(T entity);
    bool Delete(Guid id);
    bool ExistsById(Guid id);
}
