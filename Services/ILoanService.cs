using BusinessObjects;

namespace Services;

public interface ILoanService
{
    Task<Loan?> GetByIdAsync(Guid id);
    IQueryable<Loan> GetAllAsQueryable();
    Task<IEnumerable<Loan>> GetByUserAsync(Guid userId);
    Task<IEnumerable<Loan>> GetActiveByUserAsync(Guid userId);
    Task<Loan> CreateLoanAsync(Guid userId, Guid bookCopyId, Guid libraryId);
    Task<bool> ReturnBookAsync(Guid loanId);
    Task<bool> RenewLoanAsync(Guid loanId);
    Task<IEnumerable<Loan>> GetOverdueLoansAsync();
    Task<IEnumerable<Loan>> GetDueSoonAsync(int days);
    Task<IEnumerable<Loan>> GetByLibraryAsync(Guid libraryId);
    Task<bool> MarkAsOverdueAsync(Guid loanId);
    Task<int> CalculateOverdueDaysAsync(Guid loanId);
}
