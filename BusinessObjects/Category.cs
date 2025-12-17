namespace BusinessObjects;

public class Category : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}
