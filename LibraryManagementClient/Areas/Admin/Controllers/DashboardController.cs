using LibraryManagementClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementClient.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly IApiService _apiService;

    public DashboardController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        // Get dashboard statistics
        var dashboardData = await _apiService.GetAsync<dynamic>("api/reports/admin/dashboard");
        ViewBag.DashboardData = dashboardData;
        
        return View();
    }
}
