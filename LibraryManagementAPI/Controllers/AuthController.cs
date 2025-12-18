using LibraryManagementAPI.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace LibraryManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public AuthController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var (user, token) = await _authService.LoginAsync(loginDto.Email, loginDto.Password);
        
        if (user == null)
            return Unauthorized(new { message = "Invalid credentials" });

        var userWithRoles = await _userService.GetWithRolesAsync(user.Id);
        var roles = userWithRoles?.UserRoles.Select(ur => ur.Role.Name).ToList() ?? new List<string>();

        var response = new LoginResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Token = token!,
            MustChangePassword = user.MustChangePassword,
            Roles = roles,
            AssignedLibraryId = user.AssignedLibraryId
        };

        return Ok(response);
    }

    [HttpPost("magic-link")]
    public async Task<IActionResult> GenerateMagicLink([FromBody] MagicLinkRequestDto request)
    {
        var token = await _authService.GenerateMagicLinkAsync(request.UserId);
        return Ok(new { token, message = "Magic link generated successfully" });
    }

    [HttpGet("magic-link/verify")]
    public async Task<IActionResult> VerifyMagicLink([FromQuery] string token)
    {
        var user = await _authService.ValidateMagicLinkAsync(token);
        
        if (user == null)
            return BadRequest(new { message = "Invalid or expired magic link" });

        return Ok(new { userId = user.Id, message = "Magic link verified" });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] OtpRequestDto request)
    {
        var result = await _authService.GenerateOtpAsync(request.Email);
        
        if (!result)
            return BadRequest(new { message = "Email not found" });

        return Ok(new { message = "OTP sent to email" });
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyDto request)
    {
        var result = await _authService.ValidateOtpAsync(request.Email, request.Code);
        
        if (!result)
            return BadRequest(new { message = "Invalid or expired OTP" });

        return Ok(new { message = "OTP verified successfully" });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
    {
        var result = await _authService.ResetPasswordAsync(request.Email, request.OtpCode, request.NewPassword);
        
        if (!result)
            return BadRequest(new { message = "Password reset failed" });

        return Ok(new { message = "Password reset successfully" });
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
    {
        var result = await _authService.ChangePasswordAsync(request.UserId, request.NewPassword);
        
        if (!result)
            return BadRequest(new { message = "Password change failed" });

        return Ok(new { message = "Password changed successfully" });
    }
}
