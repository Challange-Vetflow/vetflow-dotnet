using VetFlow.Application.Repositories;
using VetFlow.Domain.Common;
using VetFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace VetFlow.Infrastructure.Repositories;

public class Repository<T>(VetFlowContext context) : IRepository<T> where T : BaseEntity
{
    private readonly DbSet<T> _set = context.Set<T>();

    public IReadOnlyList<T> GetAll() =>
        _set.OrderBy(e => e.CreatedAt).ToList();

    public T? GetById(Guid id) => _set.Find(id);

    public T Add(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _set.Add(entity);
        context.SaveChanges();
        return entity;
    }

    public bool Delete(Guid id)
    {
        var entity = GetById(id);
        if (entity is null) return false;
        _set.Remove(entity);
        context.SaveChanges();
        return true;
    }

    public bool ExistsById(Guid id) => _set.Count(e => e.Id == id) > 0;
}
