namespace BusinessObjects;

public class SystemSettings : BaseEntity
{
    public Guid LibraryId { get; set; }
    public Library Library { get; set; } = null!;

    public int MaxBooksPerBorrower { get; set; }
    public int LoanDurationDays { get; set; }
    public int MaxRenewals { get; set; }
    public int RenewalDays { get; set; }
    public decimal DailyFineRate { get; set; }
    public decimal LostBookFinePercent { get; set; }
    public int ReservationExpiryDays { get; set; }
}
