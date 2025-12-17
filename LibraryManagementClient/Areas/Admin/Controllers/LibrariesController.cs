using LibraryManagementClient.Models;
using LibraryManagementClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementClient.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class LibrariesController : Controller
{
    private readonly IApiService _apiService;

    public LibrariesController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        var libraries = await _apiService.GetAsync<List<LibraryDto>>("api/libraries");
        return View(libraries ?? new List<LibraryDto>());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LibraryDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _apiService.PostAsync<LibraryDto>("api/libraries", model);
        if (result != null)
        {
            TempData["Success"] = "Library created successfully.";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Failed to create library");
        return View(model);
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var library = await _apiService.GetAsync<LibraryDto>($"api/libraries/{id}");
        if (library == null)
        {
            TempData["Error"] = "Library not found.";
            return RedirectToAction(nameof(Index));
        }
        return View(library);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, LibraryDto model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(model);

        var result = await _apiService.PutAsync<LibraryDto>($"api/libraries/{id}", model);
        if (result != null)
        {
            TempData["Success"] = "Library updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Failed to update library");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _apiService.DeleteAsync($"api/libraries/{id}");
        TempData[result ? "Success" : "Error"] = result ? "Library deleted successfully." : "Failed to delete library.";
        return RedirectToAction(nameof(Index));
    }
}
