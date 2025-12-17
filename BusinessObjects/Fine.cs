namespace BusinessObjects;

public enum FineStatus
{
    Pending,
    Paid,
    Waived
}

public enum FineType
{
    Overdue,
    Lost,
    Damaged
}

public class Fine : BaseEntity
{
    public decimal Amount { get; set; }
    public FineType Type { get; set; }
    public FineStatus Status { get; set; }
    public string Reason { get; set; } = null!;
    public string? WaiverReason { get; set; }
    public Guid? WaivedBy { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid? LoanId { get; set; }
    public Loan? Loan { get; set; }

    public ICollection<FinePayment> Payments { get; set; } = new List<FinePayment>();
}
