namespace BusinessObjects;

public enum SuggestionStatus
{
    Pending,
    Approved,
    Rejected
}

public class BookPurchaseSuggestion : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string? ISBN { get; set; }
    public string Reason { get; set; } = null!;
    public SuggestionStatus Status { get; set; }
    public string? AdminNotes { get; set; }
    public string? ProcessedByLibrarianId { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public Guid LibraryId { get; set; }
    public Library Library { get; set; } = null!;
    
    public Guid? ProcessedByLibrarian { get; set; }
    public User? Processor { get; set; } = null!;
}
