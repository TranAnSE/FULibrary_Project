using BusinessObjects;

namespace Repositories;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> SoftDeleteAsync(Guid id);
    Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate);
}
