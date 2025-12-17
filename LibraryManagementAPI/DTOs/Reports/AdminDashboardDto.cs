namespace LibraryManagementAPI.DTOs.Reports;

public class AdminDashboardDto
{
    public Dictionary<Guid, LibraryStatsDto> LibraryStats { get; set; } = new();
    public List<TopBookDto> TopBorrowedBooks { get; set; } = new();
}

public class LibraryStatsDto
{
    public Guid LibraryId { get; set; }
    public string LibraryName { get; set; } = null!;
    public int TotalBooks { get; set; }
    public int AvailableCopies { get; set; }
    public int BorrowedCopies { get; set; }
    public decimal AvailabilityRatio { get; set; }
    public int TotalLoans { get; set; }
}

public class TopBookDto
{
    public string BookTitle { get; set; } = null!;
    public int LoanCount { get; set; }
    public Guid LibraryId { get; set; }
    public string LibraryName { get; set; } = null!;
}
