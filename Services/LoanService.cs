using BusinessObjects;
using DataAccessObjects;
using Repositories;

namespace Services;

public class LoanService : ILoanService
{
    private readonly ILoanRepository _loanRepository;
    private readonly IBookCopyDAO _bookCopyDAO;
    private readonly ISystemSettingsDAO _systemSettingsDAO;

    public LoanService(
        ILoanRepository loanRepository,
        IBookCopyDAO bookCopyDAO,
        ISystemSettingsDAO systemSettingsDAO)
    {
        _loanRepository = loanRepository;
        _bookCopyDAO = bookCopyDAO;
        _systemSettingsDAO = systemSettingsDAO;
    }

    public async Task<Loan?> GetByIdAsync(Guid id)
    {
        return await _loanRepository.GetWithDetailsAsync(id);
    }

    public IQueryable<Loan> GetAllAsQueryable()
    {
        return _loanRepository.GetAllAsQueryable();
    }

    public async Task<IEnumerable<Loan>> GetByUserAsync(Guid userId)
    {
        var loans = await _loanRepository.GetByUserAsync(userId);
        foreach (var loan in loans)
        {
            loan.IsOverdue = loan.ReturnDate == null && loan.DueDate < DateTime.UtcNow;
        }
        return loans;
    }

    public async Task<IEnumerable<Loan>> GetActiveByUserAsync(Guid userId)
    {
        var loans = await _loanRepository.GetActiveByUserAsync(userId);
        foreach (var loan in loans)
        {
            loan.IsOverdue = loan.ReturnDate == null && loan.DueDate < DateTime.UtcNow;
        }
        return loans;
    }

    public async Task<Loan> CreateLoanAsync(Guid userId, Guid bookCopyId, Guid libraryId)
    {
        var bookCopy = await _bookCopyDAO.GetByIdAsync(bookCopyId);
        if (bookCopy == null || bookCopy.Status != BookCopyStatus.Available)
            throw new InvalidOperationException("Book copy is not available");

        var activeCount = await _loanRepository.GetActiveCountByUserAsync(userId);
        var settings = await _systemSettingsDAO.GetByLibraryAsync(libraryId);
        
        if (settings != null && activeCount >= settings.MaxBooksPerBorrower)
            throw new InvalidOperationException("User has reached maximum books limit");

        var loanDuration = settings?.LoanDurationDays ?? 14;
        var loan = new Loan
        {
            UserId = userId,
            BookCopyId = bookCopyId,
            LibraryId = libraryId,
            LoanDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(loanDuration),
            RenewalCount = 0,
            IsOverdue = false
        };

        bookCopy.Status = BookCopyStatus.Borrowed;
        await _bookCopyDAO.UpdateAsync(bookCopy);

        return await _loanRepository.CreateAsync(loan);
    }

    public async Task<bool> ReturnBookAsync(Guid loanId)
    {
        var loan = await _loanRepository.GetWithDetailsAsync(loanId);
        if (loan == null || loan.ReturnDate.HasValue)
            return false;

        loan.ReturnDate = DateTime.UtcNow;
        
        var bookCopy = await _bookCopyDAO.GetByIdAsync(loan.BookCopyId);
        if (bookCopy != null)
        {
            bookCopy.Status = BookCopyStatus.Available;
            await _bookCopyDAO.UpdateAsync(bookCopy);
        }

        await _loanRepository.UpdateAsync(loan);
        return true;
    }

    public async Task<bool> RenewLoanAsync(Guid loanId)
    {
        var loan = await _loanRepository.GetWithDetailsAsync(loanId);
        if (loan == null || loan.ReturnDate.HasValue || loan.IsOverdue)
            return false;

        var settings = await _systemSettingsDAO.GetByLibraryAsync(loan.LibraryId);
        var maxRenewals = settings?.MaxRenewals ?? 2;
        var renewalDays = settings?.RenewalDays ?? 7;

        if (loan.RenewalCount >= maxRenewals)
            return false;

        loan.DueDate = loan.DueDate.AddDays(renewalDays);
        loan.RenewalCount++;
        await _loanRepository.UpdateAsync(loan);
        return true;
    }

    public async Task<IEnumerable<Loan>> GetOverdueLoansAsync()
    {
        var loans = await _loanRepository.GetOverdueAsync();
        foreach (var loan in loans)
        {
            loan.IsOverdue = true; // This method specifically returns overdue loans
        }
        return loans;
    }

    public async Task<IEnumerable<Loan>> GetDueSoonAsync(int days)
    {
        var loans = await _loanRepository.GetDueSoonAsync(days);
        foreach (var loan in loans)
        {
            loan.IsOverdue = loan.ReturnDate == null && loan.DueDate < DateTime.UtcNow;
        }
        return loans;
    }

    public async Task<IEnumerable<Loan>> GetByLibraryAsync(Guid libraryId)
    {
        var loans = await _loanRepository.GetByLibraryAsync(libraryId);
        foreach (var loan in loans)
        {
            loan.IsOverdue = loan.ReturnDate == null && loan.DueDate < DateTime.UtcNow;
        }
        return loans;
    }

    public async Task<bool> MarkAsOverdueAsync(Guid loanId)
    {
        var loan = await _loanRepository.GetByIdAsync(loanId);
        if (loan == null)
            return false;

        loan.IsOverdue = true;
        await _loanRepository.UpdateAsync(loan);
        return true;
    }

    public async Task<int> CalculateOverdueDaysAsync(Guid loanId)
    {
        var loan = await _loanRepository.GetByIdAsync(loanId);
        if (loan == null || loan.ReturnDate.HasValue)
            return 0;

        var today = DateTime.UtcNow.Date;
        if (loan.DueDate >= today)
            return 0;

        return (today - loan.DueDate.Date).Days;
    }
}
