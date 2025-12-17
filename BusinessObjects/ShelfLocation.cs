namespace BusinessObjects;

public class ShelfLocation : BaseEntity
{
    public string Code { get; set; } = null!;
    public string? Description { get; set; }

    public Guid LibraryId { get; set; }
    public Library Library { get; set; } = null!;

    public ICollection<BookCopy> BookCopies { get; set; } = new List<BookCopy>();
}
