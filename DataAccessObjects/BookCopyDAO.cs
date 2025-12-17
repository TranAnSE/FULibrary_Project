using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public class BookCopyDAO : BaseDAO<BookCopy>, IBookCopyDAO
{
    public BookCopyDAO(FULibraryDbContext context) : base(context)
    {
    }

    public async Task<BookCopy?> GetByRegistrationNumberAsync(string registrationNumber)
    {
        return await _dbSet
            .Include(bc => bc.Book)
            .Include(bc => bc.ShelfLocation)
            .FirstOrDefaultAsync(bc => bc.RegistrationNumber == registrationNumber);
    }

    public async Task<IEnumerable<BookCopy>> GetByBookAsync(Guid bookId)
    {
        return await _dbSet
            .Where(bc => bc.BookId == bookId)
            .Include(bc => bc.ShelfLocation)
            .ToListAsync();
    }

    public async Task<IEnumerable<BookCopy>> GetAvailableCopiesAsync(Guid bookId)
    {
        return await _dbSet
            .Where(bc => bc.BookId == bookId && bc.Status == BookCopyStatus.Available)
            .Include(bc => bc.ShelfLocation)
            .ToListAsync();
    }

    public async Task<int> GetAvailableCountAsync(Guid bookId)
    {
        return await _dbSet
            .CountAsync(bc => bc.BookId == bookId && bc.Status == BookCopyStatus.Available);
    }
}
