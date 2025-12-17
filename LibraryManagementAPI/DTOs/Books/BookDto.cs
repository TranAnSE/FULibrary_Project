namespace LibraryManagementAPI.DTOs.Books;

public class BookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string? ISBN { get; set; }
    public int? PublicationYear { get; set; }
    public string? DDC { get; set; }
    public string? Subject { get; set; }
    public string? Keyword { get; set; }
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public decimal? Price { get; set; }
    public Guid LibraryId { get; set; }
    public string? LibraryName { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public Guid? LanguageId { get; set; }
    public string? LanguageName { get; set; }
    public Guid? PublisherId { get; set; }
    public string? PublisherName { get; set; }
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
}
