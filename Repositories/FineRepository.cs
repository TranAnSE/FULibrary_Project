using BusinessObjects;
using DataAccessObjects;

namespace Repositories;

public class FineRepository : Repository<Fine>, IFineRepository
{
    private readonly IFineDAO _fineDAO;

    public FineRepository(IFineDAO fineDAO) : base(fineDAO)
    {
        _fineDAO = fineDAO;
    }

    public async Task<IEnumerable<Fine>> GetByUserAsync(Guid userId)
    {
        return await _fineDAO.GetByUserAsync(userId);
    }

    public async Task<IEnumerable<Fine>> GetPendingByUserAsync(Guid userId)
    {
        return await _fineDAO.GetPendingByUserAsync(userId);
    }

    public async Task<decimal> GetTotalPendingAmountByUserAsync(Guid userId)
    {
        return await _fineDAO.GetTotalPendingAmountByUserAsync(userId);
    }

    public async Task<Fine?> GetWithPaymentsAsync(Guid id)
    {
        return await _fineDAO.GetWithPaymentsAsync(id);
    }

    public async Task<IEnumerable<Fine>> GetByLoanAsync(Guid loanId)
    {
        return await _fineDAO.GetByLoanAsync(loanId);
    }
}
