using System.Text.Json.Serialization;

namespace LibraryManagementClient.Models;

public class AdminDashboardDto
{
    [JsonPropertyName("libraryStats")]
    public Dictionary<Guid, LibraryStatsDto> LibraryStats { get; set; } = new();
    [JsonPropertyName("topBorrowedBooks")]
    public List<TopBookDto> TopBorrowedBooks { get; set; } = new();
    [JsonPropertyName("systemStats")]
    public SystemStatsDto SystemStats { get; set; } = new();
}

public class SystemStatsDto
{
    [JsonPropertyName("totalLibraries")]
    public int TotalLibraries { get; set; }
    [JsonPropertyName("totalBooks")]
    public int TotalBooks { get; set; }
    [JsonPropertyName("activeUsers")]
    public int ActiveUsers { get; set; }
    [JsonPropertyName("activeLoans")]
    public int ActiveLoans { get; set; }
    [JsonPropertyName("pendingReservations")]
    public int PendingReservations { get; set; }
}

public class LibraryStatsDto
{
    public Guid LibraryId { get; set; }
    public string LibraryName { get; set; } = string.Empty;
    public int TotalBooks { get; set; }
    public int AvailableCopies { get; set; }
    public int BorrowedCopies { get; set; }
    public decimal AvailabilityRatio { get; set; }
    public int TotalLoans { get; set; }
}

public class TopBookDto
{
    public string BookTitle { get; set; } = string.Empty;
    public int LoanCount { get; set; }
    public Guid LibraryId { get; set; }
    public string LibraryName { get; set; } = string.Empty;
}
