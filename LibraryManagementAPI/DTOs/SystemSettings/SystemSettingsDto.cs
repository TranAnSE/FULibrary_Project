namespace LibraryManagementAPI.DTOs.SystemSettings;

public class SystemSettingsDto
{
    public Guid Id { get; set; }
    public Guid LibraryId { get; set; }
    public string LibraryName { get; set; } = null!;
    public int MaxBooksPerBorrower { get; set; }
    public int LoanDurationDays { get; set; }
    public int MaxRenewals { get; set; }
    public int RenewalDays { get; set; }
    public decimal DailyFineRate { get; set; }
    public decimal LostBookFinePercent { get; set; }
    public int ReservationExpiryDays { get; set; }
}

public class UpdateSystemSettingsDto
{
    public Guid Id { get; set; }
    public int MaxBooksPerBorrower { get; set; }
    public int LoanDurationDays { get; set; }
    public int MaxRenewals { get; set; }
    public int RenewalDays { get; set; }
    public decimal DailyFineRate { get; set; }
    public decimal LostBookFinePercent { get; set; }
    public int ReservationExpiryDays { get; set; }
}
