namespace LibraryManagementAPI.DTOs.Users;

public class CreateUserDto
{
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? IdentityCard { get; set; }
    public string? CardNumber { get; set; }
    public Guid? HomeLibraryId { get; set; }
    public Guid? AssignedLibraryId { get; set; }
    public List<string> Roles { get; set; } = new();
}
