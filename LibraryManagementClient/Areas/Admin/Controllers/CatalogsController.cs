using LibraryManagementClient.Models;
using LibraryManagementClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementClient.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
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
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(string name, string? description)
    {
        var result = await _apiService.PostAsync<CategoryDto>("api/catalogs/categories", new { name, description });
        TempData[result != null ? "Success" : "Error"] = result != null ? "Category created." : "Failed to create category.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateLanguage(string name, string? code)
    {
        var result = await _apiService.PostAsync<LanguageDto>("api/catalogs/languages", new { name, code });
        TempData[result != null ? "Success" : "Error"] = result != null ? "Language created." : "Failed to create language.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePublisher(string name, string? address)
    {
        var result = await _apiService.PostAsync<PublisherDto>("api/catalogs/publishers", new { name, address });
        TempData[result != null ? "Success" : "Error"] = result != null ? "Publisher created." : "Failed to create publisher.";
        return RedirectToAction(nameof(Index));
    }
}
