namespace LibraryManagementClient.Models;

// OData Book entity that matches server side Book entity
public class BookODataDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Author { get; set; }
    public string? ISBN { get; set; }
    public int? PublicationYear { get; set; }
    public string? DDC { get; set; }
    public string? Subject { get; set; }
    public string? Keyword { get; set; }
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public decimal? Price { get; set; }
    public Guid LibraryId { get; set; }
    public LibraryODataDto? Library { get; set; }
    public Guid? CategoryId { get; set; }
    public CategoryODataDto? Category { get; set; }
    public Guid? LanguageId { get; set; }
    public LanguageODataDto? Language { get; set; }
    public Guid? PublisherId { get; set; }
    public PublisherODataDto? Publisher { get; set; }
    public List<BookCopyODataDto>? Copies { get; set; }
}

public class LibraryODataDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CategoryODataDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class LanguageODataDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class PublisherODataDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class BookCopyODataDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class BookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Author { get; set; }
    public string? ISBN { get; set; }
    public int? PublicationYear { get; set; }
    public string? DDC { get; set; }
    public string? Subject { get; set; }
    public string? Keyword { get; set; }
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public decimal? Price { get; set; }
    public Guid LibraryId { get; set; }
    public string LibraryName { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public Guid? LanguageId { get; set; }
    public string? LanguageName { get; set; }
    public Guid? PublisherId { get; set; }
    public string? PublisherName { get; set; }
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
}

public class BookDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Author { get; set; }
    public string? ISBN { get; set; }
    public int? PublicationYear { get; set; }
    public string? DDC { get; set; }
    public string? Subject { get; set; }
    public string? Keyword { get; set; }
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public decimal? Price { get; set; }
    public Guid LibraryId { get; set; }
    public string LibraryName { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public Guid? LanguageId { get; set; }
    public string? LanguageName { get; set; }
    public Guid? PublisherId { get; set; }
    public string? PublisherName { get; set; }
    public List<BookCopyDto> Copies { get; set; } = new();
}

public class BookCopyDto
{
    public Guid Id { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid? ShelfLocationId { get; set; }
    public string? ShelfLocationCode { get; set; }
}

public class BookCopySearchDto
{
    public Guid Id { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string BookTitle { get; set; } = string.Empty;
    public string BookAuthor { get; set; } = string.Empty;
    public Guid BookId { get; set; }
}

public class SearchViewModel
{
    public string? SearchTerm { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? ISBN { get; set; }
    public string? Subject { get; set; }
    public string? Keyword { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? LanguageId { get; set; }
    public Guid? PublisherId { get; set; }
    public int? YearFrom { get; set; }
    public int? YearTo { get; set; }
    public List<BookDto> Results { get; set; } = new();
    public int TotalResults { get; set; }
}

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class LanguageDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
}

public class PublisherDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
}
