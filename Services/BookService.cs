using BusinessObjects;
using DataAccessObjects;
using Repositories;

namespace Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IBookCopyDAO _bookCopyDAO;

    public BookService(IBookRepository bookRepository, IBookCopyDAO bookCopyDAO)
    {
        _bookRepository = bookRepository;
        _bookCopyDAO = bookCopyDAO;
    }

    public async Task<Book?> GetByIdAsync(Guid id)
    {
        return await _bookRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await _bookRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Book>> GetByLibraryAsync(Guid libraryId)
    {
        return await _bookRepository.GetByLibraryAsync(libraryId);
    }

    public async Task<Book> CreateAsync(Book book)
    {
        return await _bookRepository.CreateAsync(book);
    }

    public async Task<Book> UpdateAsync(Book book)
    {
        return await _bookRepository.UpdateAsync(book);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _bookRepository.SoftDeleteAsync(id);
    }

    public async Task<Book?> GetWithCopiesAsync(Guid id)
    {
        return await _bookRepository.GetWithCopiesAsync(id);
    }

    public async Task<IEnumerable<Book>> SearchAsync(string searchTerm)
    {
        return await _bookRepository.SearchAsync(searchTerm);
    }

    public async Task<IEnumerable<Book>> GetByCategoryAsync(Guid categoryId)
    {
        return await _bookRepository.GetByCategoryAsync(categoryId);
    }

    public async Task<IEnumerable<Book>> GetNewBooksThisMonthAsync(Guid? libraryId = null)
    {
        return await _bookRepository.GetNewBooksThisMonthAsync(libraryId);
    }

    public async Task<int> GetAvailableCopiesCountAsync(Guid bookId)
    {
        return await _bookCopyDAO.GetAvailableCountAsync(bookId);
    }
}
