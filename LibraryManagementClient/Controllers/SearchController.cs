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
        // Handle null values properly for optional fields
        if (!string.IsNullOrWhiteSpace(model.Title))
            filterParts.Add($"contains(tolower(Title),'{model.Title.ToLower().Replace("'", "''")}')");
        
        if (!string.IsNullOrWhiteSpace(model.Author))
            filterParts.Add($"contains(tolower(Author),'{model.Author.ToLower().Replace("'", "''")}')");
        
        if (!string.IsNullOrWhiteSpace(model.ISBN))
            filterParts.Add($"contains(tolower(ISBN),'{model.ISBN.ToLower().Replace("'", "''")}')");
        
        if (!string.IsNullOrWhiteSpace(model.Subject))
            filterParts.Add($"contains(tolower(Subject),'{model.Subject.ToLower().Replace("'", "''")}')");
        
        if (!string.IsNullOrWhiteSpace(model.Keyword))
            filterParts.Add($"contains(tolower(Keyword),'{model.Keyword.ToLower().Replace("'", "''")}')");
        
        // Add exact filters
        if (model.CategoryId.HasValue)
            filterParts.Add($"CategoryId eq {model.CategoryId.Value}");
        
        if (model.LanguageId.HasValue)
            filterParts.Add($"LanguageId eq {model.LanguageId.Value}");
        
        if (model.PublisherId.HasValue)
            filterParts.Add($"PublisherId eq {model.PublisherId.Value}");
        
        // Add year range filters with validation logic
        if (model.YearFrom.HasValue)
        {
            filterParts.Add($"PublicationYear ge {model.YearFrom.Value}");
        }
        
        if (model.YearTo.HasValue)
        {
            filterParts.Add($"PublicationYear le {model.YearTo.Value}");
        }

        // Build OData query
        string odataQuery = "";
        if (filterParts.Any())
        {
            // URL encode the filter to handle special characters
            var filterExpression = Uri.EscapeDataString(string.Join(" and ", filterParts));
            odataQuery = $"?$filter={filterExpression}&$expand=Library,Category,Language,Publisher";
        }
        else
        {
            // If no filters, just get all books
            odataQuery = "?$expand=Library,Category,Language,Publisher";
        }

        try
        {
            // Use API endpoint with OData query parameters
            Console.WriteLine($"Making API call to: api/Books{odataQuery}");
            var results = await _apiService.GetAsync<List<BookODataDto>>($"api/Books{odataQuery}");
            Console.WriteLine($"API returned {results?.Count ?? 0} results");
            
            // Convert OData entities to client DTOs
            var bookDtos = results?.Select(book => new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                PublicationYear = book.PublicationYear,
                DDC = book.DDC,
                Subject = book.Subject,
                Keyword = book.Keyword,
                Description = book.Description,
                CoverImageUrl = book.CoverImageUrl,
                Price = book.Price,
                LibraryId = book.LibraryId,
                LibraryName = book.Library?.Name ?? "",
                CategoryId = book.CategoryId,
                CategoryName = book.Category?.Name,
                LanguageId = book.LanguageId,
                LanguageName = book.Language?.Name,
                PublisherId = book.PublisherId,
                PublisherName = book.Publisher?.Name,
                TotalCopies = book.Copies?.Count ?? 0,
                AvailableCopies = book.Copies?.Count(c => c.Status == "Available") ?? 0
            }).ToList() ?? new List<BookDto>();
            
            model.Results = bookDtos;
            model.TotalResults = model.Results.Count;
        }
        catch (Exception ex)
        {
            // Log error and return empty results
            Console.WriteLine($"Error in Advanced search: {ex.Message}");
            Console.WriteLine($"Full exception: {ex}");
            model.Results = new List<BookDto>();
            model.TotalResults = 0;
        }

        // Reload filter options
        ViewBag.Categories = await _apiService.GetAsync<List<CategoryDto>>("api/catalogs/categories") ?? new List<CategoryDto>();
        ViewBag.Languages = await _apiService.GetAsync<List<LanguageDto>>("api/catalogs/languages") ?? new List<LanguageDto>();
        ViewBag.Publishers = await _apiService.GetAsync<List<PublisherDto>>("api/catalogs/publishers") ?? new List<PublisherDto>();

        return View(model);
    }
}
