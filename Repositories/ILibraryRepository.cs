using BusinessObjects;

namespace Repositories;

public interface ILibraryRepository : IRepository<Library>
{
    Task<Library?> GetByNameAsync(string name);
    Task<Library?> GetWithBooksAsync(Guid id);
    Task<Library?> GetWithSettingsAsync(Guid id);
}
