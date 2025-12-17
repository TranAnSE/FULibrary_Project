using System.ComponentModel.DataAnnotations;

namespace LibraryManagementClient.Models;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? CardNumber { get; set; }
    public string? PhoneNumber { get; set; }
    public bool MustChangePassword { get; set; }
    public bool IsLocked { get; set; }
    public Guid? HomeLibraryId { get; set; }
    public string? HomeLibraryName { get; set; }
    public Guid? AssignedLibraryId { get; set; }
    public string? AssignedLibraryName { get; set; }
    public List<string> Roles { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class CreateUserDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full name is required")]
    public string FullName { get; set; } = string.Empty;

    public string? CardNumber { get; set; }
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Home library is required")]
    public Guid HomeLibraryId { get; set; }

    public Guid? AssignedLibraryId { get; set; }

    [Required(ErrorMessage = "At least one role is required")]
    public List<string> Roles { get; set; } = new();
}

public class UpdateUserDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full name is required")]
    public string FullName { get; set; } = string.Empty;

    public string? CardNumber { get; set; }
    public string? PhoneNumber { get; set; }
    public Guid? HomeLibraryId { get; set; }
    public Guid? AssignedLibraryId { get; set; }
}

public class LibraryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Hours { get; set; }
}
