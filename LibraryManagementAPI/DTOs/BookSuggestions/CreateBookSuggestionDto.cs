namespace LibraryManagementAPI.DTOs.BookSuggestions;

public class CreateBookSuggestionDto
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string? ISBN { get; set; }
    public string Reason { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}
