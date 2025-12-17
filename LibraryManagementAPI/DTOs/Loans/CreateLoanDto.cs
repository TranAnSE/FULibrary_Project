namespace LibraryManagementAPI.DTOs.Loans;

public class CreateLoanDto
{
    public Guid UserId { get; set; }
    public Guid BookCopyId { get; set; }
    public Guid LibraryId { get; set; }
}

public class RenewalDto
{
    public Guid LoanId { get; set; }
}
