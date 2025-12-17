using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public class BookPurchaseSuggestionDAO : BaseDAO<BookPurchaseSuggestion>, IBookPurchaseSuggestionDAO
{
    public BookPurchaseSuggestionDAO(FULibraryDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<BookPurchaseSuggestion>> GetByUserAsync(Guid userId)
    {
        return await _dbSet
            .Where(bps => bps.UserId == userId)
            .OrderByDescending(bps => bps.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<BookPurchaseSuggestion>> GetPendingAsync()
    {
        return await _dbSet
            .Where(bps => bps.Status == SuggestionStatus.Pending)
            .Include(bps => bps.User)
            .OrderBy(bps => bps.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<BookPurchaseSuggestion>> GetByStatusAsync(SuggestionStatus status)
    {
        return await _dbSet
            .Where(bps => bps.Status == status)
            .Include(bps => bps.User)
            .OrderByDescending(bps => bps.CreatedAt)
            .ToListAsync();
    }
}
