namespace BusinessObjects;

public class Book : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string? ISBN { get; set; }
    public int? PublicationYear { get; set; }
    public string? DDC { get; set; }
    public string? Subject { get; set; }
    public string? Keyword { get; set; }
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public decimal? Price { get; set; }

    public Guid LibraryId { get; set; }
    public Library Library { get; set; } = null!;

    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }

    public Guid? LanguageId { get; set; }
    public Language? Language { get; set; }

    public Guid? PublisherId { get; set; }
    public Publisher? Publisher { get; set; }

    public ICollection<BookCopy> Copies { get; set; } = new List<BookCopy>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
