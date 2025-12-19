using LibraryManagementClient.Models;
using LibraryManagementClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagementClient.Areas.Borrower.Controllers;

[Area("Borrower")]
[Authorize(Roles = "Borrower,Librarian,Admin")]
public class MyLoansController : Controller
{
    private readonly IApiService _apiService;

    public MyLoansController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Current()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var loans = await _apiService.GetAsync<List<LoanDto>>($"api/loans/user/{userId}/active");
        return View(loans ?? new List<LoanDto>());
    }

    public async Task<IActionResult> History()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var loans = await _apiService.GetAsync<List<LoanDto>>($"api/loans/user/{userId}");
        return View(loans ?? new List<LoanDto>());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Renew(Guid id)
    {
        try
        {
            await _apiService.PostAsync<object>($"api/loans/{id}/renew");
            TempData["Success"] = "Loan renewed successfully.";
        }
        catch
        {
            TempData["Error"] = "Failed to renew loan. May be overdue or at max renewals.";
        }
        return RedirectToAction(nameof(Current));
    }
}
