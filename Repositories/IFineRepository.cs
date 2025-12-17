using BusinessObjects;

namespace Repositories;

public interface IFineRepository : IRepository<Fine>
{
    Task<IEnumerable<Fine>> GetByUserAsync(Guid userId);
    Task<IEnumerable<Fine>> GetPendingByUserAsync(Guid userId);
    Task<decimal> GetTotalPendingAmountByUserAsync(Guid userId);
    Task<Fine?> GetWithPaymentsAsync(Guid id);
    Task<IEnumerable<Fine>> GetByLoanAsync(Guid loanId);
}
