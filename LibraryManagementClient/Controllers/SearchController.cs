using LibraryManagementClient.Models;
using LibraryManagementClient.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementClient.Controllers;

public class SearchController : Controller
{
    private readonly IApiService _apiService;

    public SearchController(IApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm)
    {
        var model = new SearchViewModel();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            model.SearchTerm = searchTerm;
            var results = await _apiService.GetAsync<List<BookDto>>($"api/books/search?term={Uri.EscapeDataString(searchTerm)}");
            model.Results = results ?? new List<BookDto>();
            model.TotalResults = model.Results.Count;
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Advanced()
    {
        // Get filter options
        ViewBag.Categories = await _apiService.GetAsync<List<CategoryDto>>("api/catalogs/categories") ?? new List<CategoryDto>();
        ViewBag.Languages = await _apiService.GetAsync<List<LanguageDto>>("api/catalogs/languages") ?? new List<LanguageDto>();
        ViewBag.Publishers = await _apiService.GetAsync<List<PublisherDto>>("api/catalogs/publishers") ?? new List<PublisherDto>();

        return View(new SearchViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Advanced(SearchViewModel model)
    {
        // Build query string for advanced search
        var queryParams = new List<string>();

        if (!string.IsNullOrWhiteSpace(model.Title))
            queryParams.Add($"title={Uri.EscapeDataString(model.Title)}");
        
        if (!string.IsNullOrWhiteSpace(model.Author))
            queryParams.Add($"author={Uri.EscapeDataString(model.Author)}");
        
        if (!string.IsNullOrWhiteSpace(model.ISBN))
            queryParams.Add($"isbn={Uri.EscapeDataString(model.ISBN)}");
        
        if (!string.IsNullOrWhiteSpace(model.Subject))
            queryParams.Add($"subject={Uri.EscapeDataString(model.Subject)}");
        
        if (!string.IsNullOrWhiteSpace(model.Keyword))
            queryParams.Add($"keyword={Uri.EscapeDataString(model.Keyword)}");
        
        if (model.CategoryId.HasValue)
            queryParams.Add($"categoryId={model.CategoryId}");
        
        if (model.LanguageId.HasValue)
            queryParams.Add($"languageId={model.LanguageId}");
        
        if (model.PublisherId.HasValue)
            queryParams.Add($"publisherId={model.PublisherId}");
        
        if (model.YearFrom.HasValue)
            queryParams.Add($"yearFrom={model.YearFrom}");
        
        if (model.YearTo.HasValue)
            queryParams.Add($"yearTo={model.YearTo}");

        string query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";

        // For MVP, use basic search with combined terms
        var searchTerm = string.Join(" ", new[] { model.Title, model.Author, model.Subject, model.Keyword }
            .Where(s => !string.IsNullOrWhiteSpace(s)));

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var results = await _apiService.GetAsync<List<BookDto>>($"api/books/search?term={Uri.EscapeDataString(searchTerm)}");
            model.Results = results ?? new List<BookDto>();
            
            // Apply client-side filtering for advanced criteria
            if (model.CategoryId.HasValue)
                model.Results = model.Results.Where(b => b.CategoryId == model.CategoryId).ToList();
            
            if (model.LanguageId.HasValue)
                model.Results = model.Results.Where(b => b.LanguageId == model.LanguageId).ToList();
            
            if (model.PublisherId.HasValue)
                model.Results = model.Results.Where(b => b.PublisherId == model.PublisherId).ToList();
            
            if (model.YearFrom.HasValue)
                model.Results = model.Results.Where(b => b.PublicationYear >= model.YearFrom).ToList();
            
            if (model.YearTo.HasValue)
                model.Results = model.Results.Where(b => b.PublicationYear <= model.YearTo).ToList();

            model.TotalResults = model.Results.Count;
        }

        // Reload filter options
        ViewBag.Categories = await _apiService.GetAsync<List<CategoryDto>>("api/catalogs/categories") ?? new List<CategoryDto>();
        ViewBag.Languages = await _apiService.GetAsync<List<LanguageDto>>("api/catalogs/languages") ?? new List<LanguageDto>();
        ViewBag.Publishers = await _apiService.GetAsync<List<PublisherDto>>("api/catalogs/publishers") ?? new List<PublisherDto>();

        return View(model);
    }
}
