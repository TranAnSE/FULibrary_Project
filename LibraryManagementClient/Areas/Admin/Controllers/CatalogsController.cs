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

    [HttpGet]
    public async Task<IActionResult> EditCategory(Guid id)
    {
        var category = await _apiService.GetAsync<CategoryDto>($"api/catalogs/categories/{id}");
        if (category == null)
        {
            TempData["Error"] = "Category not found.";
            return RedirectToAction(nameof(Index));
        }
        return PartialView("_EditCategoryModal", category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCategory(Guid id, string name, string? description)
    {
        var updateDto = new { Name = name, Description = description };
        var result = await _apiService.PutAsync<CategoryDto>($"api/catalogs/categories/{id}", updateDto);
        if (result != null)
        {
            TempData["Success"] = "Category updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        TempData["Error"] = "Failed to update category.";
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var result = await _apiService.DeleteAsync($"api/catalogs/categories/{id}");
        TempData[result ? "Success" : "Error"] = result ? "Category deleted successfully." : "Failed to delete category.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteLanguage(Guid id)
    {
        var result = await _apiService.DeleteAsync($"api/catalogs/languages/{id}");
        TempData[result ? "Success" : "Error"] = result ? "Language deleted successfully." : "Failed to delete language.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePublisher(Guid id)
    {
        var result = await _apiService.DeleteAsync($"api/catalogs/publishers/{id}");
        TempData[result ? "Success" : "Error"] = result ? "Publisher deleted successfully." : "Failed to delete publisher.";
        return RedirectToAction(nameof(Index));
    }
}
