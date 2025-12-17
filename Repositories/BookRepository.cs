using BusinessObjects;
using DataAccessObjects;

namespace Repositories;

public class BookRepository : Repository<Book>, IBookRepository
{
    private readonly IBookDAO _bookDAO;

    public BookRepository(IBookDAO bookDAO) : base(bookDAO)
    {
        _bookDAO = bookDAO;
    }

    public async Task<IEnumerable<Book>> GetByLibraryAsync(Guid libraryId)
    {
        return await _bookDAO.GetByLibraryAsync(libraryId);
    }

    public async Task<Book?> GetByISBNAsync(string isbn)
    {
        return await _bookDAO.GetByISBNAsync(isbn);
    }

    public async Task<Book?> GetWithCopiesAsync(Guid id)
    {
        return await _bookDAO.GetWithCopiesAsync(id);
    }

    public async Task<IEnumerable<Book>> SearchAsync(string searchTerm)
    {
        return await _bookDAO.SearchAsync(searchTerm);
    }

    public async Task<IEnumerable<Book>> GetByCategoryAsync(Guid categoryId)
    {
        return await _bookDAO.GetByCategoryAsync(categoryId);
    }

    public async Task<IEnumerable<Book>> GetNewBooksThisMonthAsync(Guid? libraryId = null)
    {
        return await _bookDAO.GetNewBooksThisMonthAsync(libraryId);
    }
}
