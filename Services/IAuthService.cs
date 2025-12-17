using BusinessObjects;

namespace Services;

public interface IAuthService
{
    Task<(User? User, string? Token)> LoginAsync(string email, string password);
    Task<string> GenerateMagicLinkAsync(Guid userId);
    Task<User?> ValidateMagicLinkAsync(string token);
    Task<bool> GenerateOtpAsync(string email);
    Task<bool> ValidateOtpAsync(string email, string code);
    Task<bool> ChangePasswordAsync(Guid userId, string newPassword);
    Task<bool> ResetPasswordAsync(string email, string otpCode, string newPassword);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}
