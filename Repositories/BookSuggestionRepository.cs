using BusinessObjects;
using DataAccessObjects;

namespace Repositories;

public class BookSuggestionRepository : Repository<BookPurchaseSuggestion>, IBookSuggestionRepository
{
    private readonly IBookPurchaseSuggestionDAO _bookPurchaseSuggestionDAO;

    public BookSuggestionRepository(IBookPurchaseSuggestionDAO bookPurchaseSuggestionDAO) : base(bookPurchaseSuggestionDAO)
    {
        _bookPurchaseSuggestionDAO = bookPurchaseSuggestionDAO;
    }

    public async Task<IEnumerable<BookPurchaseSuggestion>> GetByLibraryAsync(Guid libraryId, SuggestionStatus? status = null, string? searchTerm = null)
    {
        return await _bookPurchaseSuggestionDAO.GetByLibraryAsync(libraryId, status, searchTerm);
    }

    public async Task<bool> BulkUpdateStatusAsync(IEnumerable<Guid> suggestionIds, SuggestionStatus status, Guid processedByLibrarianId, string? adminNotes = null)
    {
        return await _bookPurchaseSuggestionDAO.BulkUpdateStatusAsync(suggestionIds, status, processedByLibrarianId, adminNotes);
    }

    public async Task<IEnumerable<BookPurchaseSuggestion>> GetByUserAsync(Guid userId)
    {
        return await _bookPurchaseSuggestionDAO.GetByUserAsync(userId);
    }

    public async Task<IEnumerable<BookPurchaseSuggestion>> GetPendingAsync()
    {
        return await _bookPurchaseSuggestionDAO.GetPendingAsync();
    }

    public async Task<IEnumerable<BookPurchaseSuggestion>> GetByStatusAsync(SuggestionStatus status)
    {
        return await _bookPurchaseSuggestionDAO.GetByStatusAsync(status);
    }
}
