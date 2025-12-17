namespace LibraryManagementAPI.DTOs.Users;

public class UpdateUserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? IdentityCard { get; set; }
    public Guid? HomeLibraryId { get; set; }
    public Guid? AssignedLibraryId { get; set; }
    public bool IsLocked { get; set; }
}
