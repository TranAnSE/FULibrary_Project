using System.Security.Claims;
using LibraryManagementClient.Models;
using LibraryManagementClient.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementClient.Controllers;

public class AuthController : Controller
{
    private readonly IApiService _apiService;

    public AuthController(IApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var loginData = new { email = model.Email, password = model.Password };
        var response = await _apiService.PostAsync<LoginResponseDto>("api/auth/login", loginData);

        if (response == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password");
            return View(model);
        }

        // Store token in session
        _apiService.SetAuthToken(response.Token);
        HttpContext.Session.SetString("UserId", response.UserId.ToString());
        HttpContext.Session.SetString("FullName", response.FullName);

        // Create claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, response.UserId.ToString()),
            new Claim(ClaimTypes.Name, response.FullName),
            new Claim(ClaimTypes.Email, response.Email)
        };

        foreach (var role in response.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = model.RememberMe,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
        };

        await HttpContext.SignInAsync(
            "CookieAuth",
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        // Check if user must change password
        if (response.MustChangePassword)
        {
            TempData["Info"] = "You must change your password before continuing.";
            return RedirectToAction("ChangePassword");
        }

        // Redirect based on role
        if (response.Roles.Contains("Admin"))
        {
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }
        else if (response.Roles.Contains("Librarian"))
        {
            return RedirectToAction("Index", "Dashboard", new { area = "Librarian" });
        }
        else if (response.Roles.Contains("Borrower"))
        {
            return RedirectToAction("Index", "MyAccount", new { area = "Borrower" });
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        _apiService.ClearAuthToken();
        HttpContext.Session.Clear();
        await HttpContext.SignOutAsync("CookieAuth");
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _apiService.PostAsync<dynamic>("api/auth/forgot-password", new { email = model.Email });

        if (result != null)
        {
            TempData["Success"] = "OTP code has been sent to your email.";
            return RedirectToAction("ResetPassword", new { email = model.Email });
        }

        ModelState.AddModelError(string.Empty, "Email not found");
        return View(model);
    }

    [HttpGet]
    public IActionResult ResetPassword(string email)
    {
        var model = new VerifyOtpViewModel { Email = email };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(VerifyOtpViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var resetData = new 
        { 
            email = model.Email, 
            otpCode = model.OtpCode, 
            newPassword = model.NewPassword 
        };

        var result = await _apiService.PostAsync<dynamic>("api/auth/reset-password", resetData);

        if (result != null)
        {
            TempData["Success"] = "Password reset successfully. You can now login.";
            return RedirectToAction("Login");
        }

        ModelState.AddModelError(string.Empty, "Invalid OTP code or reset failed");
        return View(model);
    }

    [HttpGet]
    [Authorize]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Verify current password by attempting login
        var loginTest = await _apiService.PostAsync<LoginResponseDto>(
            "api/auth/login", 
            new { email = User.FindFirstValue(ClaimTypes.Email), password = model.CurrentPassword });

        if (loginTest == null)
        {
            ModelState.AddModelError("CurrentPassword", "Current password is incorrect");
            return View(model);
        }

        var changeData = new 
        { 
            userId = Guid.Parse(userId!), 
            newPassword = model.NewPassword 
        };

        var result = await _apiService.PostAsync<dynamic>("api/auth/change-password", changeData);

        if (result != null)
        {
            TempData["Success"] = "Password changed successfully.";
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Password change failed");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> MagicLink(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login");
        }

        var result = await _apiService.GetAsync<dynamic>($"api/auth/magic-link/verify?token={token}");

        if (result != null)
        {
            TempData["Success"] = "Magic link verified! Please set your password.";
            return RedirectToAction("ChangePassword");
        }

        TempData["Error"] = "Invalid or expired magic link.";
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
