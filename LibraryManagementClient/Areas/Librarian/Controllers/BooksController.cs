using LibraryManagementClient.Models;
using LibraryManagementClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementClient.Areas.Librarian.Controllers;

[Area("Librarian")]
[Authorize(Roles = "Librarian,Admin")]
public class BooksController : Controller
{
    private readonly IApiService _apiService;

    public BooksController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index(string? searchTerm)
    {
        // API automatically filters by librarian's assigned library through middleware
        string url = "api/books";
        if (!string.IsNullOrEmpty(searchTerm))
        {
            url = $"api/books/search?term={Uri.EscapeDataString(searchTerm)}";
        }
        
        var books = await _apiService.GetAsync<List<BookDto>>(url) ?? new List<BookDto>();
        
        ViewBag.SearchTerm = searchTerm;
        return View(books);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var book = await _apiService.GetAsync<BookDetailDto>($"api/books/{id}");
        
        if (book == null)
        {
            TempData["Error"] = "Book not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(book);
    }

    public async Task<IActionResult> Create()
    {
        await LoadDropdowns();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookDto model)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdowns();
            return View(model);
        }

        var libraryId = HttpContext.Session.GetString("AssignedLibraryId");
        if (!string.IsNullOrEmpty(libraryId) && Guid.TryParse(libraryId, out var libId))
        {
            model.LibraryId = libId;
        }

        var result = await _apiService.PostAsync<BookDto>("api/books", model);
        
        if (result != null)
        {
            TempData["Success"] = "Book created successfully.";
            return RedirectToAction(nameof(Details), new { id = result.Id });
        }

        ModelState.AddModelError(string.Empty, "Failed to create book");
        await LoadDropdowns();
        return View(model);
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var book = await _apiService.GetAsync<BookDto>($"api/books/{id}");
        
        if (book == null)
        {
            TempData["Error"] = "Book not found.";
            return RedirectToAction(nameof(Index));
        }

        await LoadDropdowns();
        return View(book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, BookDto model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            await LoadDropdowns();
            return View(model);
        }

        var result = await _apiService.PutAsync<BookDto>($"api/books/{id}", model);
        
        if (result != null)
        {
            TempData["Success"] = "Book updated successfully.";
            return RedirectToAction(nameof(Details), new { id = result.Id });
        }

        ModelState.AddModelError(string.Empty, "Failed to update book");
        await LoadDropdowns();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _apiService.DeleteAsync($"api/books/{id}");
        TempData[result ? "Success" : "Error"] = result ? "Book deleted successfully." : "Failed to delete book.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDropdowns()
    {
        ViewBag.Categories = await _apiService.GetAsync<List<CategoryDto>>("api/catalogs/categories") ?? new List<CategoryDto>();
        ViewBag.Languages = await _apiService.GetAsync<List<LanguageDto>>("api/catalogs/languages") ?? new List<LanguageDto>();
        ViewBag.Publishers = await _apiService.GetAsync<List<PublisherDto>>("api/catalogs/publishers") ?? new List<PublisherDto>();
        
        var libraryId = HttpContext.Session.GetString("AssignedLibraryId");
        if (!string.IsNullOrEmpty(libraryId))
        {
            ViewBag.AssignedLibraryId = libraryId;
        }
    }
}
