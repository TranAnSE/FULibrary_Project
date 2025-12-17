using BusinessObjects;
using DataAccessObjects;

namespace Repositories;

public class LibraryRepository : Repository<Library>, ILibraryRepository
{
    private readonly ILibraryDAO _libraryDAO;

    public LibraryRepository(ILibraryDAO libraryDAO) : base(libraryDAO)
    {
        _libraryDAO = libraryDAO;
    }

    public async Task<Library?> GetByNameAsync(string name)
    {
        return await _libraryDAO.GetByNameAsync(name);
    }

    public async Task<Library?> GetWithBooksAsync(Guid id)
    {
        return await _libraryDAO.GetWithBooksAsync(id);
    }

    public async Task<Library?> GetWithSettingsAsync(Guid id)
    {
        return await _libraryDAO.GetWithSettingsAsync(id);
    }
}
