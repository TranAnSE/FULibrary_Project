namespace LibraryManagementAPI.DTOs.Books;

public class CreateBookCopyDto
{
    public Guid BookId { get; set; }
    public string RegistrationNumber { get; set; } = null!;
    public Guid? ShelfLocationId { get; set; }
}
