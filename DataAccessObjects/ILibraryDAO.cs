using BusinessObjects;

namespace DataAccessObjects;

public interface ILibraryDAO : IBaseDAO<Library>
{
    Task<Library?> GetByNameAsync(string name);
    Task<Library?> GetWithBooksAsync(Guid id);
    Task<Library?> GetWithSettingsAsync(Guid id);
}
