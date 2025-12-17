using BusinessObjects;
using DataAccessObjects;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookSuggestionsController : ControllerBase
{
    private readonly IBookPurchaseSuggestionDAO _suggestionDAO;

    public BookSuggestionsController(IBookPurchaseSuggestionDAO suggestionDAO)
    {
        _suggestionDAO = suggestionDAO;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? status)
    {
        var suggestions = await _suggestionDAO.GetAllAsync();
        
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<SuggestionStatus>(status, true, out var statusEnum))
        {
            suggestions = suggestions.Where(s => s.Status == statusEnum).ToList();
        }

        return Ok(suggestions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var suggestion = await _suggestionDAO.GetByIdAsync(id);
        if (suggestion == null)
            return NotFound();

        return Ok(suggestion);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookSuggestionDto createDto)
    {
        var suggestion = new BookPurchaseSuggestion
        {
            Title = createDto.Title,
            Author = createDto.Author,
            ISBN = createDto.ISBN,
            Reason = createDto.Reason,
            UserId = createDto.UserId,
            Status = SuggestionStatus.Pending
        };

        var created = await _suggestionDAO.AddAsync(suggestion);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateSuggestionStatusDto updateDto)
    {
        var suggestion = await _suggestionDAO.GetByIdAsync(id);
        if (suggestion == null)
            return NotFound();

        if (Enum.TryParse<SuggestionStatus>(updateDto.Status, true, out var statusEnum))
        {
            suggestion.Status = statusEnum;
        }
        
        suggestion.AdminNotes = updateDto.AdminNotes;

        await _suggestionDAO.UpdateAsync(suggestion);
        return Ok(suggestion);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _suggestionDAO.SoftDeleteAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}

public class CreateBookSuggestionDto
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string? ISBN { get; set; }
    public string Reason { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}

public class UpdateSuggestionStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string? AdminNotes { get; set; }
}
