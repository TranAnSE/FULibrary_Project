namespace LibraryManagementAPI.DTOs.Loans;

public class LoanDto
{
    public Guid Id { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public int RenewalCount { get; set; }
    public bool IsOverdue { get; set; }
    public int OverdueDays { get; set; }
    public int RenewalsRemaining { get; set; }
    
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    
    public Guid BookCopyId { get; set; }
    public string RegistrationNumber { get; set; } = null!;
    public string BookTitle { get; set; } = null!;
    public string? BookCoverImageUrl { get; set; }
    
    public Guid LibraryId { get; set; }
    public string LibraryName { get; set; } = null!;
}
