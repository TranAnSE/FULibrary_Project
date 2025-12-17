using LibraryManagementClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagementClient.Areas.Borrower.Controllers;

[Area("Borrower")]
[Authorize(Roles = "Borrower,Librarian,Admin")]
public class MyAccountController : Controller
{
    private readonly IApiService _apiService;

    public MyAccountController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Auth", new { area = "" });
        }

        var user = await _apiService.GetAsync<dynamic>($"api/users/{userId}");
        ViewBag.UserData = user;
        
        return View();
    }
}
