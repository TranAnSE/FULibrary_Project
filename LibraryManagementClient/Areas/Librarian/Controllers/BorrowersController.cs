using LibraryManagementClient.Models;
using LibraryManagementClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementClient.Areas.Librarian.Controllers;

[Area("Librarian")]
[Authorize(Roles = "Librarian,Admin")]
public class BorrowersController : Controller
{
    private readonly IApiService _apiService;

    public BorrowersController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index(string? searchTerm)
    {
        // API automatically filters by librarian's assigned library through middleware
        List<UserDto> borrowers;
        
        if (!string.IsNullOrEmpty(searchTerm))
        {
            borrowers = await _apiService.GetAsync<List<UserDto>>($"api/borrowers/search?term={Uri.EscapeDataString(searchTerm)}") ?? new List<UserDto>();
        }
        else
        {
            borrowers = await _apiService.GetAsync<List<UserDto>>("api/borrowers") ?? new List<UserDto>();
        }
        
        ViewBag.SearchTerm = searchTerm;
        return View(borrowers);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var borrower = await _apiService.GetAsync<UserDto>($"api/borrowers/{id}");
        
        if (borrower == null)
        {
            TempData["Error"] = "Borrower not found.";
            return RedirectToAction(nameof(Index));
        }

        // Get borrower's loans, reservations, and fines
        var loans = await _apiService.GetAsync<List<dynamic>>($"api/loans?userId={id}") ?? new List<dynamic>();
        var reservations = await _apiService.GetAsync<List<dynamic>>($"api/reservations/user/{id}") ?? new List<dynamic>();
        var fines = await _apiService.GetAsync<List<FineDto>>($"api/fines/user/{id}") ?? new List<FineDto>();
        
        ViewBag.Loans = loans;
        ViewBag.Reservations = reservations;
        ViewBag.Fines = fines;
        
        return View(borrower);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Lock(Guid id)
    {
        var result = await _apiService.PostAsync<dynamic>($"api/borrowers/{id}/lock");
        TempData[result != null ? "Success" : "Error"] = result != null ? "Borrower account locked successfully." : "Failed to lock account.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unlock(Guid id)
    {
        var result = await _apiService.PostAsync<dynamic>($"api/borrowers/{id}/unlock");
        TempData[result != null ? "Success" : "Error"] = result != null ? "Borrower account unlocked successfully." : "Failed to unlock account.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
