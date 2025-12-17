using LibraryManagementClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagementClient.Areas.Librarian.Controllers;

[Area("Librarian")]
[Authorize(Roles = "Librarian,Admin")]
public class DashboardController : Controller
{
    private readonly IApiService _apiService;

    public DashboardController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        // Get librarian's assigned library from claims or session
        var libraryId = HttpContext.Session.GetString("AssignedLibraryId");
        
        if (!string.IsNullOrEmpty(libraryId))
        {
            var dashboardData = await _apiService.GetAsync<dynamic>($"api/reports/librarian/dashboard?libraryId={libraryId}");
            ViewBag.DashboardData = dashboardData;
            ViewBag.LibraryId = libraryId;
        }
        
        return View();
    }
}
