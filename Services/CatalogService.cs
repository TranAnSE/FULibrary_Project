using BusinessObjects;
using Repositories;

namespace Services;

public class CatalogService<T> : ICatalogService<T> where T : BaseEntity
{
    private readonly IRepository<T> _repository;

    public CatalogService(IRepository<T> repository)
    {
        _repository = repository;
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<T> CreateAsync(T entity)
    {
        return await _repository.CreateAsync(entity);
    }

    public async Task<T> UpdateAsync(T entity)
    {
        return await _repository.UpdateAsync(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _repository.SoftDeleteAsync(id);
    }
}
