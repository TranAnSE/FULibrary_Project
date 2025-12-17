using BusinessObjects;

namespace Services;

public interface IFineService
{
    Task<Fine?> GetByIdAsync(Guid id);
    Task<IEnumerable<Fine>> GetByUserAsync(Guid userId);
    Task<IEnumerable<Fine>> GetPendingByUserAsync(Guid userId);
    Task<decimal> GetTotalPendingAmountByUserAsync(Guid userId);
    Task<Fine> CreateOverdueFineAsync(Guid loanId, int overdueDays);
    Task<Fine> CreateLostBookFineAsync(Guid loanId, decimal bookPrice);
    Task<Fine> CreateDamagedBookFineAsync(Guid loanId, decimal amount, string reason);
    Task<bool> RecordPaymentAsync(Guid fineId, decimal amount, Guid receivedBy);
    Task<bool> WaiveFineAsync(Guid fineId, string reason, Guid waivedBy);
    Task<bool> ReduceFineAsync(Guid fineId, decimal newAmount, string reason);
}
