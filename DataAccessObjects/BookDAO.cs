using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public class BookDAO : BaseDAO<Book>, IBookDAO
{
    public BookDAO(FULibraryDbContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await _dbSet
            .Include(b => b.Library)
            .Include(b => b.Category)
            .Include(b => b.Language)
            .Include(b => b.Publisher)
            .Include(b => b.Copies)
            .ToListAsync();
    }

    public new IQueryable<Book> GetAllAsQueryable()
    {
        return _dbSet
            .Include(b => b.Library)
            .Include(b => b.Category)
            .Include(b => b.Language)
            .Include(b => b.Publisher)
            .Include(b => b.Copies);
    }

    public async Task<IEnumerable<Book>> GetByLibraryAsync(Guid libraryId)
    {
        return await _dbSet
            .Where(b => b.LibraryId == libraryId)
            .Include(b => b.Library)
            .Include(b => b.Category)
            .Include(b => b.Language)
            .Include(b => b.Publisher)
            .Include(b => b.Copies)
            .ToListAsync();
    }

    public async Task<Book?> GetByISBNAsync(string isbn)
    {
        return await _dbSet.FirstOrDefaultAsync(b => b.ISBN == isbn);
    }

    public async Task<Book?> GetWithCopiesAsync(Guid id)
    {
        return await _dbSet
            .Include(b => b.Library)
            .Include(b => b.Copies)
            .ThenInclude(c => c.ShelfLocation)
            .Include(b => b.Category)
            .Include(b => b.Language)
            .Include(b => b.Publisher)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Book>> SearchAsync(string searchTerm)
    {
        var lowerSearchTerm = searchTerm.ToLower();
        return await _dbSet
            .Where(b =>
                b.Title.ToLower().Contains(lowerSearchTerm) ||
                b.Author.ToLower().Contains(lowerSearchTerm) ||
                (b.ISBN != null && b.ISBN.ToLower().Contains(lowerSearchTerm)) ||
                (b.Subject != null && b.Subject.ToLower().Contains(lowerSearchTerm)) ||
                (b.Keyword != null && b.Keyword.ToLower().Contains(lowerSearchTerm)))
            .Include(b => b.Library)
            .Include(b => b.Category)
            .Include(b => b.Language)
            .Include(b => b.Publisher)
            .Include(b => b.Copies)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetByCategoryAsync(Guid categoryId)
    {
        return await _dbSet
            .Where(b => b.CategoryId == categoryId)
            .Include(b => b.Library)
            .Include(b => b.Category)
            .Include(b => b.Language)
            .Include(b => b.Publisher)
            .Include(b => b.Copies)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetNewBooksThisMonthAsync(Guid? libraryId = null)
    {
        var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        IQueryable<Book> query = _dbSet
            .Where(b => b.CreatedAt >= startOfMonth);

        if (libraryId.HasValue)
        {
            query = query.Where(b => b.LibraryId == libraryId.Value);
        }

        return await query
            .Include(b => b.Library)
            .Include(b => b.Category)
            .Include(b => b.Language)
            .Include(b => b.Publisher)
            .Include(b => b.Copies)
            .ToListAsync();
    }
}
