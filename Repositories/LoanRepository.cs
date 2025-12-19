using BusinessObjects;
using DataAccessObjects;

namespace Repositories;

public class LoanRepository : Repository<Loan>, ILoanRepository
{
    private readonly ILoanDAO _loanDAO;

    public LoanRepository(ILoanDAO loanDAO) : base(loanDAO)
    {
        _loanDAO = loanDAO;
    }

    public new IQueryable<Loan> GetAllAsQueryable()
    {
        return _loanDAO.GetAllAsQueryable();
    }

    public async Task<IEnumerable<Loan>> GetByUserAsync(Guid userId)
    {
        return await _loanDAO.GetByUserAsync(userId);
    }

    public async Task<IEnumerable<Loan>> GetActiveByUserAsync(Guid userId)
    {
        return await _loanDAO.GetActiveByUserAsync(userId);
    }

    public async Task<IEnumerable<Loan>> GetOverdueAsync()
    {
        return await _loanDAO.GetOverdueAsync();
    }

    public async Task<IEnumerable<Loan>> GetDueSoonAsync(int days)
    {
        return await _loanDAO.GetDueSoonAsync(days);
    }

    public async Task<IEnumerable<Loan>> GetByLibraryAsync(Guid libraryId)
    {
        return await _loanDAO.GetByLibraryAsync(libraryId);
    }

    public async Task<Loan?> GetWithDetailsAsync(Guid id)
    {
        return await _loanDAO.GetWithDetailsAsync(id);
    }

    public async Task<int> GetActiveCountByUserAsync(Guid userId)
    {
        return await _loanDAO.GetActiveCountByUserAsync(userId);
    }
}
