namespace BusinessObjects;

public class Language : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Code { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}
