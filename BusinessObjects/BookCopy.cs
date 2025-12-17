namespace BusinessObjects;

public enum BookCopyStatus
{
    Available,
    Borrowed,
    Lost,
    Damaged
}

public class BookCopy : BaseEntity
{
    public string RegistrationNumber { get; set; } = null!;
    public BookCopyStatus Status { get; set; }

    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    public Guid? ShelfLocationId { get; set; }
    public ShelfLocation? ShelfLocation { get; set; }

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
