namespace LibraryManagementAPI.DTOs.Fines;

public class FinePaymentDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string? Notes { get; set; }
    public Guid ReceivedBy { get; set; }
    public string? ReceivedByName { get; set; }
}

public class CreateFinePaymentDto
{
    public Guid FineId { get; set; }
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
    public Guid ReceivedBy { get; set; }
}

public class WaiverDto
{
    public Guid FineId { get; set; }
    public string Reason { get; set; } = null!;
    public Guid WaivedBy { get; set; }
}

public class ReduceFineDto
{
    public Guid FineId { get; set; }
    public decimal NewAmount { get; set; }
    public string Reason { get; set; } = null!;
}
