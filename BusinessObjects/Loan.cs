namespace BusinessObjects;

public class Loan : BaseEntity
{
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public int RenewalCount { get; set; }
    public bool IsOverdue { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid BookCopyId { get; set; }
    public BookCopy BookCopy { get; set; } = null!;

    public Guid LibraryId { get; set; }
    public Library? Library { get; set; }

    public ICollection<Fine> Fines { get; set; } = new List<Fine>();
}
