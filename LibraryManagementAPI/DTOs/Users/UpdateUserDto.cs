namespace LibraryManagementAPI.DTOs.Users;

public class UpdateUserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? IdentityCard { get; set; }
    public string? CardNumber { get; set; }
    public Guid? HomeLibraryId { get; set; }
    public Guid? AssignedLibraryId { get; set; }
    public bool IsLocked { get; set; }
    public List<string> Roles { get; set; } = new();
}
