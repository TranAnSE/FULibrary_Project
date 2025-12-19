using AutoMapper;
using BusinessObjects;
using LibraryManagementAPI.DTOs.Catalogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Services;

namespace LibraryManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "Librarian")]
public class CatalogsController : ControllerBase
{
    private readonly ICatalogService<Category> _categoryService;
    private readonly ICatalogService<Language> _languageService;
    private readonly ICatalogService<Publisher> _publisherService;
    private readonly IShelfLocationRepository _shelfLocationRepository;
    private readonly IMapper _mapper;

    public CatalogsController(
        ICatalogService<Category> categoryService,
        ICatalogService<Language> languageService,
        ICatalogService<Publisher> publisherService,
        IShelfLocationRepository shelfLocationRepository,
        IMapper mapper)
    {
        _categoryService = categoryService;
        _languageService = languageService;
        _publisherService = publisherService;
        _shelfLocationRepository = shelfLocationRepository;
        _mapper = mapper;
    }

    // Categories
    [HttpGet("categories")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _categoryService.GetAllAsync();
        var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);
        return Ok(categoryDtos);
    }

    [HttpGet("categories/{id}")]
    public async Task<IActionResult> GetCategory(Guid id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null)
            return NotFound();

        var categoryDto = _mapper.Map<CategoryDto>(category);
        return Ok(categoryDto);
    }

    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createDto)
    {
        var category = _mapper.Map<Category>(createDto);
        var created = await _categoryService.CreateAsync(category);
        var categoryDto = _mapper.Map<CategoryDto>(created);
        return CreatedAtAction(nameof(GetCategory), new { id = created.Id }, categoryDto);
    }

    [HttpPut("categories/{id}")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] CreateCategoryDto updateDto)
    {
        var existing = await _categoryService.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        _mapper.Map(updateDto, existing);
        await _categoryService.UpdateAsync(existing);
        return Ok(_mapper.Map<CategoryDto>(existing));
    }

    [HttpDelete("categories/{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var result = await _categoryService.DeleteAsync(id);
        if (!result)
            return NotFound();
        return NoContent();
    }

    // Languages
    [HttpGet("languages")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLanguages()
    {
        var languages = await _languageService.GetAllAsync();
        var languageDtos = _mapper.Map<List<LanguageDto>>(languages);
        return Ok(languageDtos);
    }

    [HttpPost("languages")]
    public async Task<IActionResult> CreateLanguage([FromBody] CreateLanguageDto createDto)
    {
        var language = _mapper.Map<Language>(createDto);
        var created = await _languageService.CreateAsync(language);
        return Ok(_mapper.Map<LanguageDto>(created));
    }

    [HttpDelete("languages/{id}")]
    public async Task<IActionResult> DeleteLanguage(Guid id)
    {
        var result = await _languageService.DeleteAsync(id);
        if (!result)
            return NotFound();
        return NoContent();
    }

    // Publishers
    [HttpGet("publishers")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublishers()
    {
        var publishers = await _publisherService.GetAllAsync();
        var publisherDtos = _mapper.Map<List<PublisherDto>>(publishers);
        return Ok(publisherDtos);
    }

    [HttpPost("publishers")]
    public async Task<IActionResult> CreatePublisher([FromBody] CreatePublisherDto createDto)
    {
        var publisher = _mapper.Map<Publisher>(createDto);
        var created = await _publisherService.CreateAsync(publisher);
        return Ok(_mapper.Map<PublisherDto>(created));
    }

    [HttpDelete("publishers/{id}")]
    public async Task<IActionResult> DeletePublisher(Guid id)
    {
        var result = await _publisherService.DeleteAsync(id);
        if (!result)
            return NotFound();
        return NoContent();
    }

    // Shelf Locations
    [HttpGet("shelf-locations")]
    public async Task<IActionResult> GetShelfLocations([FromQuery] Guid? libraryId)
    {
        IEnumerable<ShelfLocation> locations;
        
        if (libraryId.HasValue)
            locations = await _shelfLocationRepository.GetByLibraryAsync(libraryId.Value);
        else
            locations = await _shelfLocationRepository.GetAllAsync();

        var locationDtos = _mapper.Map<List<ShelfLocationDto>>(locations);
        return Ok(locationDtos);
    }

    [HttpPost("shelf-locations")]
    public async Task<IActionResult> CreateShelfLocation([FromBody] CreateShelfLocationDto createDto)
    {
        var location = _mapper.Map<ShelfLocation>(createDto);
        var created = await _shelfLocationRepository.CreateAsync(location);
        return Ok(_mapper.Map<ShelfLocationDto>(created));
    }
}
