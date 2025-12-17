using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public class LoanDAO : BaseDAO<Loan>, ILoanDAO
{
    public LoanDAO(FULibraryDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Loan>> GetByUserAsync(Guid userId)
    {
        return await _dbSet
            .Where(l => l.UserId == userId)
            .Include(l => l.BookCopy)
            .ThenInclude(bc => bc.Book)
            .OrderByDescending(l => l.LoanDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Loan>> GetActiveByUserAsync(Guid userId)
    {
        return await _dbSet
            .Where(l => l.UserId == userId && l.ReturnDate == null)
            .Include(l => l.BookCopy)
            .ThenInclude(bc => bc.Book)
            .OrderBy(l => l.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Loan>> GetOverdueAsync()
    {
        var today = DateTime.UtcNow.Date;
        return await _dbSet
            .Where(l => l.ReturnDate == null && l.DueDate < today)
            .Include(l => l.User)
            .Include(l => l.BookCopy)
            .ThenInclude(bc => bc.Book)
            .ToListAsync();
    }

    public async Task<IEnumerable<Loan>> GetDueSoonAsync(int days)
    {
        var today = DateTime.UtcNow.Date;
        var dueDate = today.AddDays(days);
        return await _dbSet
            .Where(l => l.ReturnDate == null && l.DueDate >= today && l.DueDate <= dueDate)
            .Include(l => l.User)
            .Include(l => l.BookCopy)
            .ThenInclude(bc => bc.Book)
            .OrderBy(l => l.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Loan>> GetByLibraryAsync(Guid libraryId)
    {
        return await _dbSet
            .Where(l => l.LibraryId == libraryId)
            .Include(l => l.User)
            .Include(l => l.BookCopy)
            .ThenInclude(bc => bc.Book)
            .OrderByDescending(l => l.LoanDate)
            .ToListAsync();
    }

    public async Task<Loan?> GetWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(l => l.User)
            .Include(l => l.BookCopy)
            .ThenInclude(bc => bc.Book)
            .ThenInclude(b => b.Category)
            .Include(l => l.Fines)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<int> GetActiveCountByUserAsync(Guid userId)
    {
        return await _dbSet
            .CountAsync(l => l.UserId == userId && l.ReturnDate == null);
    }
}
