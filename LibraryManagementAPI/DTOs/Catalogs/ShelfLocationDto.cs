namespace LibraryManagementAPI.DTOs.Catalogs;

public class ShelfLocationDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string? Description { get; set; }
    public Guid LibraryId { get; set; }
    public string LibraryName { get; set; } = null!;
}

public class CreateShelfLocationDto
{
    public string Code { get; set; } = null!;
    public string? Description { get; set; }
    public Guid LibraryId { get; set; }
}
