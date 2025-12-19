namespace LibraryManagementAPI.DTOs.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? IdentityCard { get; set; }
    public string? CardNumber { get; set; }
    public DateTime? CardIssuedDate { get; set; }
    public DateTime? CardExpiryDate { get; set; }
    public bool MustChangePassword { get; set; }
    public bool IsLocked { get; set; }
    public Guid? HomeLibraryId { get; set; }
    public string? HomeLibraryName { get; set; }
    public Guid? AssignedLibraryId { get; set; }
    public string? AssignedLibraryName { get; set; }
    public List<string> Roles { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
