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
        // Get filter options - ensure they are loaded properly
        var categories = await _apiService.GetAsync<List<CategoryDto>>("api/catalogs/categories") ?? new List<CategoryDto>();
        var languages = await _apiService.GetAsync<List<LanguageDto>>("api/catalogs/languages") ?? new List<LanguageDto>();
        var publishers = await _apiService.GetAsync<List<PublisherDto>>("api/catalogs/publishers") ?? new List<PublisherDto>();

        ViewBag.Categories = categories;
        ViewBag.Languages = languages;
        ViewBag.Publishers = publishers;

        return View(new SearchViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Advanced(SearchViewModel model)
    {
        // Build OData query for advanced search with combined conditions
        var filterParts = new List<string>();

        // Add text-based filters with contains condition (case-insensitive)
        if (!string.IsNullOrWhiteSpace(model.Title))
            filterParts.Add($"contains(tolower(title),'{model.Title.ToLower()}')");
        
        if (!string.IsNullOrWhiteSpace(model.Author))
            filterParts.Add($"contains(tolower(author),'{model.Author.ToLower()}')");
        
        if (!string.IsNullOrWhiteSpace(model.ISBN))
            filterParts.Add($"contains(tolower(isbn),'{model.ISBN.ToLower()}')");
        
        if (!string.IsNullOrWhiteSpace(model.Subject))
            filterParts.Add($"contains(tolower(subject),'{model.Subject.ToLower()}')");
        
        if (!string.IsNullOrWhiteSpace(model.Keyword))
            filterParts.Add($"contains(tolower(keyword),'{model.Keyword.ToLower()}')");
        
        // Add exact filters
        if (model.CategoryId.HasValue)
            filterParts.Add($"categoryId eq {model.CategoryId.Value}");
        
        if (model.LanguageId.HasValue)
            filterParts.Add($"languageId eq {model.LanguageId.Value}");
        
        if (model.PublisherId.HasValue)
            filterParts.Add($"publisherId eq {model.PublisherId.Value}");
        
        // Add year range filters with validation logic
        if (model.YearFrom.HasValue)
        {
            filterParts.Add($"publicationYear ge {model.YearFrom.Value}");
        }
        
        if (model.YearTo.HasValue)
        {
            filterParts.Add($"publicationYear le {model.YearTo.Value}");
        }

        // Build OData query
        string odataQuery = "";
        if (filterParts.Any())
        {
            odataQuery = $"?$filter={string.Join(" and ", filterParts)}&$expand=Library,Category,Language,Publisher";
        }
        else
        {
            odataQuery = "?$expand=Library,Category,Language,Publisher";
        }

        // Use OData endpoint for advanced filtering
        var results = await _apiService.GetAsync<List<BookDto>>($"odata/Books{odataQuery}");
        model.Results = results ?? new List<BookDto>();
        model.TotalResults = model.Results.Count;

        // Reload filter options
        ViewBag.Categories = await _apiService.GetAsync<List<CategoryDto>>("api/catalogs/categories") ?? new List<CategoryDto>();
        ViewBag.Languages = await _apiService.GetAsync<List<LanguageDto>>("api/catalogs/languages") ?? new List<LanguageDto>();
        ViewBag.Publishers = await _apiService.GetAsync<List<PublisherDto>>("api/catalogs/publishers") ?? new List<PublisherDto>();

        return View(model);
    }
}
