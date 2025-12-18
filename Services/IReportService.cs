namespace Services;

public interface IReportService
{
    Task<Dictionary<Guid, int>> GetTotalBooksByLibraryAsync();
    Task<Dictionary<Guid, List<(string BookTitle, int LoanCount)>>> GetTopBorrowedBooksByLibraryAsync(int topCount = 5);
    Task<Dictionary<Guid, (int Available, int Borrowed)>> GetAvailabilityRatioByLibraryAsync();
    Task<int> GetTodayLoansCountAsync(Guid libraryId);
    Task<IEnumerable<(string BookTitle, DateTime DueDate, string BorrowerName)>> GetBooksDueSoonAsync(Guid libraryId, int days);
    Task<IEnumerable<(string BookTitle, int OverdueDays, string BorrowerName)>> GetOverdueLoansAsync(Guid libraryId);
    Task<int> GetBorrowerActiveLoansCountAsync(string userId);
    Task<int> GetBorrowerReservationsCountAsync(string userId);
    Task<IEnumerable<decimal>> GetBorrowerPendingFinesAsync(string userId);
    Task<int> GetPendingReservationsCountAsync(Guid libraryId);
    Task<Dictionary<string, int>> GetSystemWideStatisticsAsync();
}
