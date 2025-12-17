using BusinessObjects;

namespace DataAccessObjects;

public interface IBookPurchaseSuggestionDAO : IBaseDAO<BookPurchaseSuggestion>
{
    Task<IEnumerable<BookPurchaseSuggestion>> GetByUserAsync(Guid userId);
    Task<IEnumerable<BookPurchaseSuggestion>> GetPendingAsync();
    Task<IEnumerable<BookPurchaseSuggestion>> GetByStatusAsync(SuggestionStatus status);
}
