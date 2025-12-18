using LibraryManagementClient.Models;
using LibraryManagementClient.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementClient.Controllers;

public class BooksController : Controller
{
    private readonly IApiService _apiService;

    public BooksController(IApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var book = await _apiService.GetAsync<BookDetailDto>($"api/books/{id}");

            if (book == null)
            {
                TempData["Error"] = "Book not found.";
                return RedirectToAction("Index", "Home");
            }

            // For guests, hide detailed availability information
            ViewBag.ShowAvailability = User.Identity?.IsAuthenticated == true;

            return View(book);
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Unable to fetch book details. Please try again later.";
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpGet]
    public async Task<IActionResult> New()
    {
        var newBooks = await _apiService.GetAsync<List<BookDto>>("api/books/new");
        ViewBag.NewBooks = newBooks ?? new List<BookDto>();
        
        return View();
    }
}
