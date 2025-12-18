using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public class BookPurchaseSuggestionDAO : BaseDAO<BookPurchaseSuggestion>, IBookPurchaseSuggestionDAO
{
    public BookPurchaseSuggestionDAO(FULibraryDbContext context) : base(context)
    {
    }

    public override async Task<BookPurchaseSuggestion?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(bps => bps.User)
            .Include(bps => bps.Library)
            .Include(bps => bps.Processor)
            .FirstOrDefaultAsync(bps => bps.Id == id && !bps.IsDeleted);
    }

    public async Task<IEnumerable<BookPurchaseSuggestion>> GetByUserAsync(Guid userId)
    {
        return await _dbSet
            .Where(bps => bps.UserId == userId && !bps.IsDeleted)
            .OrderByDescending(bps => bps.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<BookPurchaseSuggestion>> GetPendingAsync()
    {
        return await _dbSet
            .Where(bps => bps.Status == SuggestionStatus.Pending && !bps.IsDeleted)
            .Include(bps => bps.User)
            .OrderBy(bps => bps.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<BookPurchaseSuggestion>> GetByStatusAsync(SuggestionStatus status)
    {
        return await _dbSet
            .Where(bps => bps.Status == status && !bps.IsDeleted)
            .Include(bps => bps.User)
            .OrderByDescending(bps => bps.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<BookPurchaseSuggestion>> GetByLibraryAsync(Guid libraryId, SuggestionStatus? status = null, string? searchTerm = null)
    {
        var query = _dbSet
            .Include(bps => bps.User)
            .Include(bps => bps.Processor)
            .Where(bps => bps.LibraryId == libraryId && !bps.IsDeleted);

        if (status.HasValue)
        {
            query = query.Where(bps => bps.Status == status.Value);
        }

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(bps =>
                bps.Title.Contains(searchTerm) ||
                bps.Author.Contains(searchTerm) ||
                bps.User!.FullName!.Contains(searchTerm));
        }

        return await query.OrderByDescending(bps => bps.CreatedAt).ToListAsync();
    }

    public async Task<bool> BulkUpdateStatusAsync(IEnumerable<Guid> suggestionIds, SuggestionStatus status, Guid processedByLibrarianId, string? adminNotes = null)
    {
        try
        {
            var suggestions = await _dbSet
                .Where(bps => suggestionIds.Contains(bps.Id))
                .ToListAsync();

            foreach (var suggestion in suggestions)
            {
                suggestion.Status = status;
                suggestion.ProcessedByLibrarian = processedByLibrarianId;
                
                if (!string.IsNullOrEmpty(adminNotes))
                {
                    suggestion.AdminNotes = adminNotes;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
