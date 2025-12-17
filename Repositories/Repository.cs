using BusinessObjects;
using DataAccessObjects;

namespace Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly IBaseDAO<T> _dao;

    public Repository(IBaseDAO<T> dao)
    {
        _dao = dao;
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dao.GetByIdAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dao.GetAllAsync();
    }

    public async Task<T> CreateAsync(T entity)
    {
        return await _dao.AddAsync(entity);
    }

    public async Task<T> UpdateAsync(T entity)
    {
        return await _dao.UpdateAsync(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _dao.DeleteAsync(id);
    }

    public async Task<bool> SoftDeleteAsync(Guid id)
    {
        return await _dao.SoftDeleteAsync(id);
    }

    public async Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate)
    {
        return await _dao.FindAsync(predicate);
    }
}
