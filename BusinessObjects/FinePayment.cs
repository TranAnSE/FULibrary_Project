namespace BusinessObjects;

public class FinePayment : BaseEntity
{
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string? Notes { get; set; }

    public Guid FineId { get; set; }
    public Fine Fine { get; set; } = null!;

    public Guid ReceivedBy { get; set; }
}
