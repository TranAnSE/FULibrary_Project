using BusinessObjects;
using LibraryManagementClient.Models;
using LibraryManagementClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementClient.Areas.Librarian.Controllers;

[Area("Librarian")]
[Authorize(Roles = "Librarian,Admin")]
public class BookSuggestionsController : Controller
{
    private readonly IApiService _apiService;

    public BookSuggestionsController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index(SuggestionStatus? status = null, string? searchTerm = null)
    {
        try
        {
            var queryParams = new List<string>();
            
            if (status.HasValue)
                queryParams.Add($"status={status.Value}");
            
            if (!string.IsNullOrEmpty(searchTerm))
                queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");

            var queryString = queryParams.Any() ? $"?{string.Join("&", queryParams)}" : "";
            var suggestions = await _apiService.GetAsync<List<BookSuggestionDto>>($"api/librarianbooksuggestions{queryString}") 
                             ?? new List<BookSuggestionDto>();
            
            // Pass filter values back to view
            ViewBag.Status = status;
            ViewBag.SearchTerm = searchTerm;
            
            return View(suggestions);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Failed to load suggestions: {ex.Message}";
            return View(new List<BookSuggestionDto>());
        }
    }

    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var suggestion = await _apiService.GetAsync<BookSuggestionDto>($"api/librarianbooksuggestions/{id}");
            
            if (suggestion == null)
            {
                TempData["Error"] = "Suggestion not found";
                return RedirectToAction(nameof(Index));
            }

            return View(suggestion);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Failed to load suggestion details: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public IActionResult UpdateStatus(Guid id)
    {
        var model = new UpdateSuggestionStatusDto { Id = id };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateSuggestionStatusDto model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var updateDto = new
            {
                Status = model.Status.ToString(),
                AdminNotes = model.AdminNotes
            };

            var result = await _apiService.PutAsync<BookSuggestionDto>($"api/librarianbooksuggestions/{id}/status", updateDto);
            
            if (result != null)
            {
                TempData["Success"] = "Suggestion status updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Failed to update suggestion status");
            return View(model);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Error updating status: {ex.Message}");
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BulkUpdateStatus(BulkUpdateViewModel model)
    {
        if (!model.SelectedSuggestionIds.Any())
        {
            TempData["Error"] = "Please select at least one suggestion";
            return RedirectToAction(nameof(Index));
        }

        if (!model.Status.HasValue)
        {
            TempData["Error"] = "Please select a status";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var bulkUpdateDto = new
            {
                SuggestionIds = model.SelectedSuggestionIds,
                Status = model.Status.Value.ToString(),
                AdminNotes = model.AdminNotes
            };

            await _apiService.PutAsync<object>("api/librarianbooksuggestions/bulk-status", bulkUpdateDto);
            TempData["Success"] = $"Updated {model.SelectedSuggestionIds.Count()} suggestions successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error updating suggestions: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }
}

public class UpdateSuggestionStatusDto
{
    public Guid Id { get; set; }
    public SuggestionStatus Status { get; set; }
    public string? AdminNotes { get; set; }
}

public class BulkUpdateViewModel
{
    public List<Guid> SelectedSuggestionIds { get; set; } = new List<Guid>();
    public SuggestionStatus? Status { get; set; }
    public string? AdminNotes { get; set; }
}
