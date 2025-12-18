namespace LibraryManagementAPI.DTOs.Auth;

public class LoginResponseDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Token { get; set; } = null!;
    public bool MustChangePassword { get; set; }
    public List<string> Roles { get; set; } = new();
    public Guid? AssignedLibraryId { get; set; }
}
