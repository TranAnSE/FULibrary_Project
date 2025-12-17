namespace LibraryManagementAPI.DTOs.Catalogs;

public class PublisherDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public string? Website { get; set; }
}

public class CreatePublisherDto
{
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public string? Website { get; set; }
}
