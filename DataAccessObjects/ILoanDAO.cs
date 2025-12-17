using BusinessObjects;

namespace DataAccessObjects;

public interface ILoanDAO : IBaseDAO<Loan>
{
    Task<IEnumerable<Loan>> GetByUserAsync(Guid userId);
    Task<IEnumerable<Loan>> GetActiveByUserAsync(Guid userId);
    Task<IEnumerable<Loan>> GetOverdueAsync();
    Task<IEnumerable<Loan>> GetDueSoonAsync(int days);
    Task<IEnumerable<Loan>> GetByLibraryAsync(Guid libraryId);
    Task<Loan?> GetWithDetailsAsync(Guid id);
    Task<int> GetActiveCountByUserAsync(Guid userId);
}
