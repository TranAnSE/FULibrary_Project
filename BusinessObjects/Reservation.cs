namespace BusinessObjects;

public enum ReservationStatus
{
    Pending,
    Fulfilled,
    Cancelled,
    Expired
}

public class Reservation : BaseEntity
{
    public DateTime ReservationDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime? FulfilledDate { get; set; }
    public DateTime? CancelledDate { get; set; }
    public string? Notes { get; set; }
    public ReservationStatus Status { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;
}
