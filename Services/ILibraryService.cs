using BusinessObjects;

namespace Services;

public interface ILibraryService
{
    Task<Library?> GetByIdAsync(Guid id);
    Task<IEnumerable<Library>> GetAllAsync();
    IQueryable<Library> GetAllAsQueryable();
    Task<Library> CreateAsync(Library library);
    Task<Library> UpdateAsync(Library library);
    Task<bool> DeleteAsync(Guid id);
    Task<Library?> GetWithBooksAsync(Guid id);
    Task<Library?> GetWithSettingsAsync(Guid id);
}
