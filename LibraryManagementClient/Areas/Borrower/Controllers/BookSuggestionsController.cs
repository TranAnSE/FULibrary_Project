using LibraryManagementClient.Models;
using LibraryManagementClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagementClient.Areas.Borrower.Controllers;

[Area("Borrower")]
[Authorize(Roles = "Borrower,Librarian,Admin")]
public class BookSuggestionsController : Controller
{
    private readonly IApiService _apiService;

    public BookSuggestionsController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public IActionResult Create()
    {
        return View(new CreateBookSuggestionDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBookSuggestionDto model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
        {
            TempData["Error"] = "Invalid user session.";
            return View(model);
        }

        model.UserId = userGuid;
        var result = await _apiService.PostAsync<BookSuggestionDto>("api/booksuggestions", model);
        
        if (result != null)
        {
            TempData["Success"] = "Book suggestion submitted successfully!";
            return RedirectToAction(nameof(MyRequests));
        }

        ModelState.AddModelError(string.Empty, "Failed to submit suggestion");
        return View(model);
    }

    public async Task<IActionResult> MyRequests()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var suggestions = await _apiService.GetAsync<List<BookSuggestionDto>>($"api/booksuggestions?userId={userId}") ?? new List<BookSuggestionDto>();
        
        return View(suggestions);
    }
}
