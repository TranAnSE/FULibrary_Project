namespace LibraryManagementAPI.DTOs.Libraries;

public class UpdateLibraryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Website { get; set; }
    public string? WeekdayHours { get; set; }
    public string? WeekendHours { get; set; }
    public string? SocialMedia { get; set; }
}
