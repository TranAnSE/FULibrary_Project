using LibraryManagementClient.Models;
using LibraryManagementClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

    [HttpPost]
    [Authorize(Roles = "Borrower")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reserve(Guid bookId)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var createReservationDto = new
            {
                UserId = Guid.Parse(userId!),
                BookId = bookId
            };

            var result = await _apiService.PostAsync<dynamic>("api/reservations", createReservationDto);

            TempData["Success"] = "Book reserved successfully! Please pick it up within 3 days.";
            return RedirectToAction("Current", "MyReservations", new { area = "Borrower" });
        }
        catch (HttpRequestException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Details), new { id = bookId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = "An unexpected error occurred. Please try again later.";
            return RedirectToAction(nameof(Details), new { id = bookId });
        }
    }
}
