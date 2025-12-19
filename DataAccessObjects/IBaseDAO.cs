using BusinessObjects;

namespace DataAccessObjects;

public interface IBaseDAO<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    IQueryable<T> GetAllAsQueryable();
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> SoftDeleteAsync(Guid id);
    Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate);
}
