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
            // Get new materials for display on home page
            var newBooks = await _apiService.GetAsync<List<BookDto>>("api/books/new");
            ViewBag.NewBooks = newBooks ?? new List<BookDto>();
            
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
