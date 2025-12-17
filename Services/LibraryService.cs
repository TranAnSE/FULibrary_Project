using BusinessObjects;
using Repositories;

namespace Services;

public class LibraryService : ILibraryService
{
    private readonly ILibraryRepository _libraryRepository;

    public LibraryService(ILibraryRepository libraryRepository)
    {
        _libraryRepository = libraryRepository;
    }

    public async Task<Library?> GetByIdAsync(Guid id)
    {
        return await _libraryRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Library>> GetAllAsync()
    {
        return await _libraryRepository.GetAllAsync();
    }

    public async Task<Library> CreateAsync(Library library)
    {
        return await _libraryRepository.CreateAsync(library);
    }

    public async Task<Library> UpdateAsync(Library library)
    {
        return await _libraryRepository.UpdateAsync(library);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _libraryRepository.SoftDeleteAsync(id);
    }

    public async Task<Library?> GetWithBooksAsync(Guid id)
    {
        return await _libraryRepository.GetWithBooksAsync(id);
    }

    public async Task<Library?> GetWithSettingsAsync(Guid id)
    {
        return await _libraryRepository.GetWithSettingsAsync(id);
    }
}
