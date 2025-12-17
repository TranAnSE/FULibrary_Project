namespace LibraryManagementAPI.DTOs.Catalogs;

public class LanguageDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Code { get; set; }
}

public class CreateLanguageDto
{
    public string Name { get; set; } = null!;
    public string? Code { get; set; }
}
