using BusinessObjects;

namespace Repositories;

public interface IBookRepository : IRepository<Book>
{
    Task<IEnumerable<Book>> GetByLibraryAsync(Guid libraryId);
    new IQueryable<Book> GetAllAsQueryable();
    Task<Book?> GetByISBNAsync(string isbn);
    Task<Book?> GetWithCopiesAsync(Guid id);
    Task<IEnumerable<Book>> SearchAsync(string searchTerm);
    Task<IEnumerable<Book>> GetByCategoryAsync(Guid categoryId);
    Task<IEnumerable<Book>> GetNewBooksThisMonthAsync(Guid? libraryId = null);
}
