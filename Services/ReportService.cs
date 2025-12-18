using BusinessObjects;
using Repositories;

namespace Services;

public class ReportService : IReportService
{
    private readonly IBookRepository _bookRepository;
    private readonly ILoanRepository _loanRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IFineRepository _fineRepository;
    private readonly IUserRepository _userRepository;

    public ReportService(IBookRepository bookRepository, ILoanRepository loanRepository, 
        IReservationRepository reservationRepository, IFineRepository fineRepository,
        IUserRepository userRepository)
    {
        _bookRepository = bookRepository;
        _loanRepository = loanRepository;
        _reservationRepository = reservationRepository;
        _fineRepository = fineRepository;
        _userRepository = userRepository;
    }

    public async Task<Dictionary<Guid, int>> GetTotalBooksByLibraryAsync()
    {
        var allBooks = await _bookRepository.GetAllAsync();
        return allBooks
            .GroupBy(b => b.LibraryId)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public async Task<Dictionary<Guid, List<(string BookTitle, int LoanCount)>>> GetTopBorrowedBooksByLibraryAsync(int topCount = 5)
    {
        var allLoans = (await _loanRepository.GetAllAsync()).ToList();
        var result = new Dictionary<Guid, List<(string BookTitle, int LoanCount)>>();

        var loansByLibrary = allLoans.GroupBy(l => l.LibraryId);
        
        foreach (var libraryGroup in loansByLibrary)
        {
            var topBooks = libraryGroup
                .GroupBy(l => new { l.BookCopy.Book.Title, l.BookCopy.BookId })
                .Select(g => (g.Key.Title, LoanCount: g.Count()))
                .OrderByDescending(x => x.LoanCount)
                .Take(topCount)
                .ToList();
            
            result[libraryGroup.Key] = topBooks;
        }

        return result;
    }

    public async Task<Dictionary<Guid, (int Available, int Borrowed)>> GetAvailabilityRatioByLibraryAsync()
    {
        var allBooks = (await _bookRepository.GetAllAsync()).ToList();
        var result = new Dictionary<Guid, (int Available, int Borrowed)>();

        foreach (var libraryGroup in allBooks.GroupBy(b => b.LibraryId))
        {
            var libraryId = libraryGroup.Key;
            var totalCopies = libraryGroup.SelectMany(b => b.Copies).ToList();
            var available = totalCopies.Count(c => c.Status == BookCopyStatus.Available);
            var borrowed = totalCopies.Count(c => c.Status == BookCopyStatus.Borrowed);
            
            result[libraryId] = (available, borrowed);
        }

        return result;
    }

    public async Task<int> GetTodayLoansCountAsync(Guid libraryId)
    {
        var loans = await _loanRepository.GetByLibraryAsync(libraryId);
        var today = DateTime.UtcNow.Date;
        return loans.Count(l => l.LoanDate.Date == today);
    }

    public async Task<IEnumerable<(string BookTitle, DateTime DueDate, string BorrowerName)>> GetBooksDueSoonAsync(Guid libraryId, int days)
    {
        var loans = await _loanRepository.GetDueSoonAsync(days);
        return loans
            .Where(l => l.LibraryId == libraryId)
            .Select(l => (
                l.BookCopy.Book.Title,
                l.DueDate,
                l.User.FullName
            ));
    }

    public async Task<IEnumerable<(string BookTitle, int OverdueDays, string BorrowerName)>> GetOverdueLoansAsync(Guid libraryId)
    {
        var loans = await _loanRepository.GetOverdueAsync();
        var today = DateTime.UtcNow.Date;
        
        return loans
            .Where(l => l.LibraryId == libraryId)
            .Select(l => (
                l.BookCopy.Book.Title,
                OverdueDays: (today - l.DueDate.Date).Days,
                l.User.FullName
            ));
    }

    public async Task<int> GetBorrowerActiveLoansCountAsync(string userId)
    {
        var userGuid = Guid.Parse(userId);
        var loans = await _loanRepository.GetByUserAsync(userGuid);
        return loans.Count(l => l.ReturnDate == null);
    }

    public async Task<int> GetBorrowerReservationsCountAsync(string userId)
    {
        var userGuid = Guid.Parse(userId);
        var reservations = await _reservationRepository.GetActiveByUserAsync(userGuid);
        return reservations.Count();
    }

    public async Task<IEnumerable<decimal>> GetBorrowerPendingFinesAsync(string userId)
    {
        var userGuid = Guid.Parse(userId);
        var pendingFines = await _fineRepository.GetPendingByUserAsync(userGuid);
        return pendingFines.Select(f => f.Amount);
    }

    public async Task<int> GetPendingReservationsCountAsync(Guid libraryId)
    {
        var pendingReservations = await _reservationRepository.GetPendingAsync();
        return pendingReservations.Count(r => r.Book.LibraryId == libraryId);
    }

    public async Task<Dictionary<string, int>> GetSystemWideStatisticsAsync()
    {
        var allBooks = await _bookRepository.GetAllAsync();
        var allLoans = await _loanRepository.GetAllAsync();
        var pendingReservations = await _reservationRepository.GetPendingAsync();
        var allUsers = await _userRepository.GetAllAsync();
        
        return new Dictionary<string, int>
        {
            ["TotalBooks"] = allBooks.Count(),
            ["ActiveLoans"] = allLoans.Count(l => l.ReturnDate == null),
            ["PendingReservations"] = pendingReservations.Count(),
            ["Libraries"] = allBooks.Select(b => b.LibraryId).Distinct().Count(),
            ["ActiveUsers"] = allUsers.Count(u => !u.IsLocked)
        };
    }
}
