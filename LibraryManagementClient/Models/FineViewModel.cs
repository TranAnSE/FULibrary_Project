namespace LibraryManagementClient.Models;

public class FineDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string? WaiverReason { get; set; }
    public Guid? WaivedBy { get; set; }
    public string? WaivedByName { get; set; }
    
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    
    public Guid? LoanId { get; set; }
    public string? BookTitle { get; set; }
    
    public decimal TotalPaid { get; set; }
    public decimal Balance { get; set; }
    public List<FinePaymentDto> Payments { get; set; } = new();
}

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
    public string Reason { get; set; } = string.Empty;
    public Guid WaivedBy { get; set; }
}

public class ReduceFineDto
{
    public Guid FineId { get; set; }
    public decimal NewAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
}
