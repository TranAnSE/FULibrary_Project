namespace BusinessObjects;

public class Publisher : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public string? Website { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}
