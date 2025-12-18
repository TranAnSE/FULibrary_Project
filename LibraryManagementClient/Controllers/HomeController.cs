using LibraryManagementClient.Models;
using LibraryManagementClient.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LibraryManagementClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IApiService _apiService;

        public HomeController(ILogger<HomeController> logger, IApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Test API connection first
                _logger.LogInformation("Attempting to fetch new books from API");
                var newBooks = await _apiService.GetAsync<List<BookDto>>("api/books/new");
                
                if (newBooks == null)
                {
                    _logger.LogWarning("API returned null for new books");
                    ViewBag.NewBooks = new List<BookDto>();
                }
                else
                {
                    _logger.LogInformation("Successfully fetched {Count} new books", newBooks.Count);
                    ViewBag.NewBooks = newBooks;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching new books from API");
                ViewBag.NewBooks = new List<BookDto>();
                TempData["Error"] = "Unable to connect to the library service. Please try again later.";
            }
            
            return View();
        }

        [HttpGet]
        public IActionResult Search(string? term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return RedirectToAction("Index", "Search");
            }

            return RedirectToAction("Index", "Search", new { searchTerm = term });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
