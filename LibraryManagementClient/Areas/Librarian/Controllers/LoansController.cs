using LibraryManagementClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementClient.Areas.Librarian.Controllers;

[Area("Librarian")]
[Authorize(Roles = "Librarian,Admin")]
public class LoansController : Controller
{
    private readonly IApiService _apiService;

    public LoansController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        var libraryId = HttpContext.Session.GetString("AssignedLibraryId");
        if (string.IsNullOrEmpty(libraryId))
        {
            TempData["Error"] = "No library assigned.";
            return RedirectToAction("Index", "Dashboard");
        }

        var loans = await _apiService.GetAsync<List<dynamic>>($"api/loans?libraryId={libraryId}");
        return View(loans ?? new List<dynamic>());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Guid userId, Guid bookCopyId)
    {
        var libraryId = HttpContext.Session.GetString("AssignedLibraryId");
        var result = await _apiService.PostAsync<dynamic>("api/loans", new { userId, bookCopyId, libraryId });
        
        if (result != null)
        {
            TempData["Success"] = "Loan created successfully.";
            return RedirectToAction(nameof(Index));
        }

        TempData["Error"] = "Failed to create loan.";
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Return(Guid id)
    {
        var result = await _apiService.PostAsync<dynamic>($"api/loans/{id}/return");
        TempData[result != null ? "Success" : "Error"] = result != null ? "Book returned successfully." : "Failed to return book.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Renew(Guid id)
    {
        var result = await _apiService.PostAsync<dynamic>($"api/loans/{id}/renew");
        TempData[result != null ? "Success" : "Error"] = result != null ? "Loan renewed successfully." : "Failed to renew loan.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Overdue()
    {
        var loans = await _apiService.GetAsync<List<dynamic>>("api/loans/overdue");
        return View(loans ?? new List<dynamic>());
    }
}
