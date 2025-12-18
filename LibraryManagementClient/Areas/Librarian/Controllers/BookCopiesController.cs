using LibraryManagementClient.Models;
using LibraryManagementClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementClient.Areas.Librarian.Controllers;

[Area("Librarian")]
[Authorize(Roles = "Librarian,Admin")]
public class BookCopiesController : Controller
{
    private readonly IApiService _apiService;

    public BookCopiesController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index(Guid bookId)
    {
        var copies = await _apiService.GetAsync<List<BookCopyDto>>($"api/bookcopies/book/{bookId}") ?? new List<BookCopyDto>();
        var book = await _apiService.GetAsync<BookDto>($"api/books/{bookId}");
        
        ViewBag.Book = book;
        ViewBag.BookId = bookId;
        return View(copies);
    }

    public async Task<IActionResult> Create(Guid bookId)
    {
        var book = await _apiService.GetAsync<BookDto>($"api/books/{bookId}");
        if (book == null)
        {
            TempData["Error"] = "Book not found.";
            return RedirectToAction("Index", "Books");
        }

        ViewBag.Book = book;
        await LoadShelfLocations();
        
        return View(new BookCopyDto { });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Guid bookId, string registrationNumber, Guid? shelfLocationId)
    {
        var createDto = new
        {
            BookId = bookId,
            RegistrationNumber = registrationNumber,
            ShelfLocationId = shelfLocationId,
            Status = "Available"
        };

        var result = await _apiService.PostAsync<BookCopyDto>("api/bookcopies", createDto);
        
        if (result != null)
        {
            TempData["Success"] = "Book copy added successfully.";
            return RedirectToAction("Details", "Books", new { id = bookId });
        }

        TempData["Error"] = "Failed to add book copy.";
        var book = await _apiService.GetAsync<BookDto>($"api/books/{bookId}");
        ViewBag.Book = book;
        await LoadShelfLocations();
        return View(new BookCopyDto { });
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var copy = await _apiService.GetAsync<BookCopyDto>($"api/bookcopies/{id}");
        if (copy == null)
        {
            TempData["Error"] = "Book copy not found.";
            return RedirectToAction("Index", "Books");
        }

        await LoadShelfLocations();
        return View(copy);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, BookCopyDto model)
    {
        if (id != model.Id)
            return BadRequest();

        var result = await _apiService.PutAsync<BookCopyDto>($"api/bookcopies/{id}/status", new { Status = model.Status });
        
        if (result != null)
        {
            TempData["Success"] = "Book copy updated successfully.";
            return RedirectToAction("Index", "Books");
        }

        ModelState.AddModelError(string.Empty, "Failed to update book copy");
        await LoadShelfLocations();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _apiService.DeleteAsync($"api/bookcopies/{id}");
        TempData[result ? "Success" : "Error"] = result ? "Book copy deleted successfully." : "Failed to delete book copy.";
        return RedirectToAction("Index", "Books");
    }

    private async Task LoadShelfLocations()
    {
        var libraryId = HttpContext.Session.GetString("AssignedLibraryId");
        string url = "api/catalogs/shelf-locations";
        
        if (!string.IsNullOrEmpty(libraryId))
        {
            url += $"?libraryId={libraryId}";
        }
        
        ViewBag.ShelfLocations = await _apiService.GetAsync<List<ShelfLocationDto>>(url) ?? new List<ShelfLocationDto>();
    }
}
