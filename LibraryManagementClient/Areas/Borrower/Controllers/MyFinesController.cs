using LibraryManagementClient.Models;
using LibraryManagementClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagementClient.Areas.Borrower.Controllers;

[Area("Borrower")]
[Authorize(Roles = "Borrower,Librarian,Admin")]
public class MyFinesController : Controller
{
    private readonly IApiService _apiService;

    public MyFinesController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var fines = await _apiService.GetAsync<List<FineDto>>($"api/fines/user/{userId}") ?? new List<FineDto>();
        
        var totalPending = await _apiService.GetAsync<dynamic>($"api/fines/user/{userId}/total");
        ViewBag.TotalPending = totalPending?.totalPending ?? 0m;
        
        return View(fines);
    }
}
