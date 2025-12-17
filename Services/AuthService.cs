using BusinessObjects;
using Repositories;

namespace Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public AuthService(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<(User? User, string? Token)> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || user.IsLocked)
            return (null, null);

        if (!VerifyPassword(password, user.PasswordHash))
            return (null, null);

        var userWithRoles = await _userRepository.GetWithRolesAsync(user.Id);
        var roles = userWithRoles?.UserRoles.Select(ur => ur.Role.Name).ToList() ?? new List<string>();
        
        var token = _jwtService.GenerateToken(user.Id, user.Email, user.FullName, roles, user.AssignedLibraryId);

        return (user, token);
    }

    public async Task<string> GenerateMagicLinkAsync(Guid userId)
    {
        var token = Guid.NewGuid().ToString("N");
        // TODO: Save MagicLinkToken to database
        return token;
    }

    public async Task<User?> ValidateMagicLinkAsync(string token)
    {
        // TODO: Validate magic link token from database
        return null;
    }

    public async Task<bool> GenerateOtpAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            return false;

        var otpCode = new Random().Next(100000, 999999).ToString();
        // TODO: Save OTP to database
        // TODO: Send OTP via email
        return true;
    }

    public async Task<bool> ValidateOtpAsync(string email, string code)
    {
        // TODO: Validate OTP from database
        return false;
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, string newPassword)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return false;

        user.PasswordHash = HashPassword(newPassword);
        user.MustChangePassword = false;
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<bool> ResetPasswordAsync(string email, string otpCode, string newPassword)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            return false;

        var isValidOtp = await ValidateOtpAsync(email, otpCode);
        if (!isValidOtp)
            return false;

        user.PasswordHash = HashPassword(newPassword);
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public string HashPassword(string password)
    {
        // TODO: Use BCrypt for password hashing
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        // TODO: Use BCrypt for password verification
        var hash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        return hash == hashedPassword;
    }
}
