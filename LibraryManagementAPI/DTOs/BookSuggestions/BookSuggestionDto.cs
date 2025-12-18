using BusinessObjects;

namespace LibraryManagementAPI.DTOs.BookSuggestions;

public class BookSuggestionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string? ISBN { get; set; }
    public string Reason { get; set; } = string.Empty;
    public SuggestionStatus Status { get; set; }
    public string? AdminNotes { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid LibraryId { get; set; }
    public string LibraryName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
