namespace BusinessObjects;

public class User : BaseEntity
{
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? IdentityCard { get; set; }
    public string? CardNumber { get; set; }
    public DateTime? CardIssuedDate { get; set; }
    public DateTime? CardExpiryDate { get; set; }
    public bool MustChangePassword { get; set; }
    public bool IsLocked { get; set; }

    public Guid? HomeLibraryId { get; set; }
    public Library? HomeLibrary { get; set; }

    public Guid? AssignedLibraryId { get; set; }
    public Library? AssignedLibrary { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public ICollection<Fine> Fines { get; set; } = new List<Fine>();
    public ICollection<BookPurchaseSuggestion> BookSuggestions { get; set; } = new List<BookPurchaseSuggestion>();
}
