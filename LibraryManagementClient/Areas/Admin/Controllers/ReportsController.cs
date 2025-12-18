using LibraryManagementClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementClient.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ReportsController : Controller
{
    private readonly IApiService _apiService;

    public ReportsController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var dashboardData = await _apiService.GetAsync<dynamic>("api/reports/admin/dashboard");
            ViewBag.DashboardData = dashboardData;
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Failed to load dashboard data: {ex.Message}";
            ViewBag.DashboardData = null;
        }

        return View();
    }
}
