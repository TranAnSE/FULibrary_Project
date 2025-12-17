using BusinessObjects;
using DataAccessObjects;
using Repositories;

namespace Services;

public class FineService : IFineService
{
    private readonly IFineRepository _fineRepository;
    private readonly ILoanRepository _loanRepository;
    private readonly ISystemSettingsDAO _systemSettingsDAO;

    public FineService(
        IFineRepository fineRepository,
        ILoanRepository loanRepository,
        ISystemSettingsDAO systemSettingsDAO)
    {
        _fineRepository = fineRepository;
        _loanRepository = loanRepository;
        _systemSettingsDAO = systemSettingsDAO;
    }

    public async Task<Fine?> GetByIdAsync(Guid id)
    {
        return await _fineRepository.GetWithPaymentsAsync(id);
    }

    public async Task<IEnumerable<Fine>> GetByUserAsync(Guid userId)
    {
        return await _fineRepository.GetByUserAsync(userId);
    }

    public async Task<IEnumerable<Fine>> GetPendingByUserAsync(Guid userId)
    {
        return await _fineRepository.GetPendingByUserAsync(userId);
    }

    public async Task<decimal> GetTotalPendingAmountByUserAsync(Guid userId)
    {
        return await _fineRepository.GetTotalPendingAmountByUserAsync(userId);
    }

    public async Task<Fine> CreateOverdueFineAsync(Guid loanId, int overdueDays)
    {
        var loan = await _loanRepository.GetWithDetailsAsync(loanId);
        if (loan == null)
            throw new InvalidOperationException("Loan not found");

        var settings = await _systemSettingsDAO.GetByLibraryAsync(loan.LibraryId);
        var dailyRate = settings?.DailyFineRate ?? 5000m;
        var amount = overdueDays * dailyRate;

        var fine = new Fine
        {
            UserId = loan.UserId,
            LoanId = loanId,
            Amount = amount,
            Type = FineType.Overdue,
            Status = FineStatus.Pending,
            Reason = $"Overdue by {overdueDays} days at {dailyRate:N0} VND/day"
        };

        return await _fineRepository.CreateAsync(fine);
    }

    public async Task<Fine> CreateLostBookFineAsync(Guid loanId, decimal bookPrice)
    {
        var loan = await _loanRepository.GetWithDetailsAsync(loanId);
        if (loan == null)
            throw new InvalidOperationException("Loan not found");

        var settings = await _systemSettingsDAO.GetByLibraryAsync(loan.LibraryId);
        var percentage = settings?.LostBookFinePercent ?? 100m;
        var amount = bookPrice * (percentage / 100m);

        var fine = new Fine
        {
            UserId = loan.UserId,
            LoanId = loanId,
            Amount = amount,
            Type = FineType.Lost,
            Status = FineStatus.Pending,
            Reason = $"Lost book fine at {percentage}% of book price ({bookPrice:N0} VND)"
        };

        return await _fineRepository.CreateAsync(fine);
    }

    public async Task<Fine> CreateDamagedBookFineAsync(Guid loanId, decimal amount, string reason)
    {
        var loan = await _loanRepository.GetWithDetailsAsync(loanId);
        if (loan == null)
            throw new InvalidOperationException("Loan not found");

        var fine = new Fine
        {
            UserId = loan.UserId,
            LoanId = loanId,
            Amount = amount,
            Type = FineType.Damaged,
            Status = FineStatus.Pending,
            Reason = reason
        };

        return await _fineRepository.CreateAsync(fine);
    }

    public async Task<bool> RecordPaymentAsync(Guid fineId, decimal amount, Guid receivedBy)
    {
        var fine = await _fineRepository.GetWithPaymentsAsync(fineId);
        if (fine == null)
            return false;

        var finePayment = new FinePayment
        {
            FineId = fineId,
            Amount = amount,
            PaymentDate = DateTime.UtcNow,
            ReceivedBy = receivedBy
        };

        var totalPaid = fine.Payments.Sum(p => p.Amount) + amount;
        if (totalPaid >= fine.Amount)
        {
            fine.Status = FineStatus.Paid;
        }

        await _fineRepository.UpdateAsync(fine);
        return true;
    }

    public async Task<bool> WaiveFineAsync(Guid fineId, string reason, Guid waivedBy)
    {
        var fine = await _fineRepository.GetByIdAsync(fineId);
        if (fine == null)
            return false;

        fine.Status = FineStatus.Waived;
        fine.WaiverReason = reason;
        fine.WaivedBy = waivedBy;
        await _fineRepository.UpdateAsync(fine);
        return true;
    }

    public async Task<bool> ReduceFineAsync(Guid fineId, decimal newAmount, string reason)
    {
        var fine = await _fineRepository.GetByIdAsync(fineId);
        if (fine == null || newAmount >= fine.Amount)
            return false;

        fine.Amount = newAmount;
        fine.WaiverReason = reason;
        await _fineRepository.UpdateAsync(fine);
        return true;
    }
}
