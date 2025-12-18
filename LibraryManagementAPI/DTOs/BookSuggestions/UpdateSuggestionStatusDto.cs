namespace LibraryManagementAPI.DTOs.BookSuggestions;

public class UpdateSuggestionStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string? AdminNotes { get; set; }
}
