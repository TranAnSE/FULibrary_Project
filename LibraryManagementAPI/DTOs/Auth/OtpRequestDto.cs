namespace LibraryManagementAPI.DTOs.Auth;

public class OtpRequestDto
{
    public string Email { get; set; } = null!;
}

public class OtpVerifyDto
{
    public string Email { get; set; } = null!;
    public string Code { get; set; } = null!;
}
