using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public class FineDAO : BaseDAO<Fine>, IFineDAO
{
    public FineDAO(FULibraryDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Fine>> GetByUserAsync(Guid userId)
    {
        return await _dbSet
            .Where(f => f.UserId == userId)
            .Include(f => f.Loan)
            .ThenInclude(l => l.BookCopy!)
            .ThenInclude(bc => bc.Book)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Fine>> GetPendingByUserAsync(Guid userId)
    {
        return await _dbSet
            .Where(f => f.UserId == userId && f.Status == FineStatus.Pending)
            .Include(f => f.Loan)
            .ThenInclude(l => l.BookCopy!)
            .ThenInclude(bc => bc.Book)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalPendingAmountByUserAsync(Guid userId)
    {
        return await _dbSet
            .Where(f => f.UserId == userId && f.Status == FineStatus.Pending)
            .SumAsync(f => f.Amount);
    }

    public async Task<Fine?> GetWithPaymentsAsync(Guid id)
    {
        return await _dbSet
            .Include(f => f.Payments)
            .Include(f => f.User)
            .Include(f => f.Loan)
            .ThenInclude(l => l.BookCopy!)
            .ThenInclude(bc => bc.Book)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<IEnumerable<Fine>> GetByLoanAsync(Guid loanId)
    {
        return await _dbSet
            .Where(f => f.LoanId == loanId)
            .Include(f => f.Payments)
            .ToListAsync();
    }
}
