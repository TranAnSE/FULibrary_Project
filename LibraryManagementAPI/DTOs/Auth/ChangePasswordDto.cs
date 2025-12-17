namespace LibraryManagementAPI.DTOs.Auth;

public class ChangePasswordDto
{
    public Guid UserId { get; set; }
    public string NewPassword { get; set; } = null!;
}

public class ResetPasswordDto
{
    public string Email { get; set; } = null!;
    public string OtpCode { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
