using System.Text.Json.Serialization;

namespace LibraryManagementAPI.DTOs.Reports;

public class LibrarianDashboardDto
{
    [JsonPropertyName("todayLoansCount")]
    public int TodayLoansCount { get; set; }
    [JsonPropertyName("booksDueSoon")]
    public List<DueSoonBookDto> BooksDueSoon { get; set; } = new();
    [JsonPropertyName("overdueBooks")]
    public List<OverdueBookDto> OverdueBooks { get; set; } = new();
    [JsonPropertyName("pendingReservationsCount")]
    public int PendingReservationsCount { get; set; }
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
