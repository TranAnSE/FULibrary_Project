using System.Text.Json.Serialization;

namespace LibraryManagementAPI.DTOs.Reports;

public class BorrowerDashboardDto
{
    [JsonPropertyName("activeLoansCount")]
    public int ActiveLoansCount { get; set; }
    [JsonPropertyName("currentReservationsCount")]
    public int CurrentReservationsCount { get; set; }
    [JsonPropertyName("pendingFinesTotal")]
    public decimal PendingFinesTotal { get; set; }
    [JsonPropertyName("pendingFinesCount")]
    public int PendingFinesCount { get; set; }
}
