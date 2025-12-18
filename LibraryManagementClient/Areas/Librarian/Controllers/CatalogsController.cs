using LibraryManagementClient.Models;
using LibraryManagementClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementClient.Areas.Librarian.Controllers;

[Area("Librarian")]
[Authorize(Roles = "Librarian,Admin")]
public class CatalogsController : Controller
{
    private readonly IApiService _apiService;

    public CatalogsController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Categories = await _apiService.GetAsync<List<CategoryDto>>("api/catalogs/categories") ?? new List<CategoryDto>();
        ViewBag.Languages = await _apiService.GetAsync<List<LanguageDto>>("api/catalogs/languages") ?? new List<LanguageDto>();
        ViewBag.Publishers = await _apiService.GetAsync<List<PublisherDto>>("api/catalogs/publishers") ?? new List<PublisherDto>();
        
        var libraryId = HttpContext.Session.GetString("AssignedLibraryId");
        string url = "api/catalogs/shelf-locations";
        if (!string.IsNullOrEmpty(libraryId))
        {
            url += $"?libraryId={libraryId}";
        }
        ViewBag.ShelfLocations = await _apiService.GetAsync<List<ShelfLocationDto>>(url) ?? new List<ShelfLocationDto>();
        
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateShelfLocation(string code, string? description)
    {
        var libraryId = HttpContext.Session.GetString("AssignedLibraryId");
        
        if (string.IsNullOrEmpty(libraryId) || !Guid.TryParse(libraryId, out var libId))
        {
            TempData["Error"] = "No library assigned to your account.";
            return RedirectToAction(nameof(Index));
        }

        var createDto = new { Code = code, Description = description, LibraryId = libId };
        var result = await _apiService.PostAsync<ShelfLocationDto>("api/catalogs/shelf-locations", createDto);
        
        TempData[result != null ? "Success" : "Error"] = result != null ? "Shelf location created." : "Failed to create shelf location.";
        return RedirectToAction(nameof(Index));
    }
}
