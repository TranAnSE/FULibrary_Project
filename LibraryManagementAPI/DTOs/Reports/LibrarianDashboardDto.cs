namespace LibraryManagementAPI.DTOs.Reports;

public class LibrarianDashboardDto
{
    public int TodayLoansCount { get; set; }
    public List<DueSoonBookDto> BooksDueSoon { get; set; } = new();
    public List<OverdueBookDto> OverdueBooks { get; set; } = new();
}

public class DueSoonBookDto
{
    public string BookTitle { get; set; } = null!;
    public DateTime DueDate { get; set; }
    public string BorrowerName { get; set; } = null!;
    public int DaysUntilDue { get; set; }
}

public class OverdueBookDto
{
    public string BookTitle { get; set; } = null!;
    public int OverdueDays { get; set; }
    public string BorrowerName { get; set; } = null!;
}
