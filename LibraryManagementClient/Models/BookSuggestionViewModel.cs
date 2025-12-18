using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BusinessObjects;

namespace LibraryManagementClient.Models;

public class BookSuggestionDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Author is required")]
    public string Author { get; set; } = string.Empty;

    public string? ISBN { get; set; }

    [Required(ErrorMessage = "Reason is required")]
    public string Reason { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SuggestionStatus Status { get; set; }
    public string? AdminNotes { get; set; }

    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;

    public Guid LibraryId { get; set; }
    public string LibraryName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}

public class CreateBookSuggestionDto
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Author is required")]
    [StringLength(200, ErrorMessage = "Author cannot exceed 200 characters")]
    public string Author { get; set; } = string.Empty;
    
    [StringLength(20, ErrorMessage = "ISBN cannot exceed 20 characters")]
    public string? ISBN { get; set; }
    
    [Required(ErrorMessage = "Reason is required")]
    [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
    public string Reason { get; set; } = string.Empty;
    
    public Guid UserId { get; set; }
}
