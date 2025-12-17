namespace BusinessObjects;

public class Library : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Website { get; set; }
    public string? WeekdayHours { get; set; }
    public string? WeekendHours { get; set; }
    public string? SocialMedia { get; set; }

    public ICollection<User> AssignedLibrarians { get; set; } = new List<User>();
    public ICollection<User> HomeBorrowers { get; set; } = new List<User>();
    public ICollection<Book> Books { get; set; } = new List<Book>();
    public ICollection<SystemSettings> Settings { get; set; } = new List<SystemSettings>();
    public ICollection<ShelfLocation> ShelfLocations { get; set; } = new List<ShelfLocation>();
}
