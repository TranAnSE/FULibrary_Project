namespace LibraryManagementClient.Models;

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
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;

    public Guid BookCopyId { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string BookTitle { get; set; } = string.Empty;
    public string? BookCoverImageUrl { get; set; }

    public Guid LibraryId { get; set; }
    public string LibraryName { get; set; } = string.Empty;
}
