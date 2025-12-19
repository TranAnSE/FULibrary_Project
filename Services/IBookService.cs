using BusinessObjects;

namespace Services;

public interface IBookService
{
    Task<Book?> GetByIdAsync(Guid id);
    Task<IEnumerable<Book>> GetAllAsync();
    IQueryable<Book> GetAllAsQueryable();
    Task<IEnumerable<Book>> GetByLibraryAsync(Guid libraryId);
    Task<Book> CreateAsync(Book book);
    Task<Book> UpdateAsync(Book book);
    Task<bool> DeleteAsync(Guid id);
    Task<Book?> GetWithCopiesAsync(Guid id);
    Task<IEnumerable<Book>> SearchAsync(string searchTerm);
    Task<IEnumerable<Book>> GetByCategoryAsync(Guid categoryId);
    Task<IEnumerable<Book>> GetNewBooksThisMonthAsync(Guid? libraryId = null);
    Task<int> GetAvailableCopiesCountAsync(Guid bookId);
}
