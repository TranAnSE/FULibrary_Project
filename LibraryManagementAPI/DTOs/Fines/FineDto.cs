namespace LibraryManagementAPI.DTOs.Fines;

public class FineDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string Reason { get; set; } = null!;
    public string? WaiverReason { get; set; }
    public Guid? WaivedBy { get; set; }
    public string? WaivedByName { get; set; }
    
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    
    public Guid? LoanId { get; set; }
    public string? BookTitle { get; set; }
    
    public decimal TotalPaid { get; set; }
    public decimal Balance { get; set; }
    public List<FinePaymentDto> Payments { get; set; } = new();
}
