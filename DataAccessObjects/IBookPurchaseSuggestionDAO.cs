using BusinessObjects;

namespace DataAccessObjects;

public interface IBookPurchaseSuggestionDAO : IBaseDAO<BookPurchaseSuggestion>
{
    Task<IEnumerable<BookPurchaseSuggestion>> GetByUserAsync(Guid userId);
    Task<IEnumerable<BookPurchaseSuggestion>> GetPendingAsync();
    Task<IEnumerable<BookPurchaseSuggestion>> GetByStatusAsync(SuggestionStatus status);
    Task<IEnumerable<BookPurchaseSuggestion>> GetByLibraryAsync(Guid libraryId, SuggestionStatus? status = null, string? searchTerm = null);
    Task<bool> BulkUpdateStatusAsync(IEnumerable<Guid> suggestionIds, SuggestionStatus status, Guid processedByLibrarianId, string? adminNotes = null);
}
