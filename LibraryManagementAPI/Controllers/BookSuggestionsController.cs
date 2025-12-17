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
    public async Task<IActionResult> GetAll([FromQuery] Guid? libraryId, [FromQuery] string? status)
    {
        var suggestions = await _suggestionDAO.GetAllAsync();
        
        if (libraryId.HasValue)
            suggestions = suggestions.Where(s => s.LibraryId == libraryId.Value).ToList();
        
        if (!string.IsNullOrEmpty(status))
            suggestions = suggestions.Where(s => s.Status.ToLower() == status.ToLower()).ToList();

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
            BookTitle = createDto.BookTitle,
            Author = createDto.Author,
            ISBN = createDto.ISBN,
            Reason = createDto.Reason,
            SuggestedBy = createDto.SuggestedBy,
            LibraryId = createDto.LibraryId,
            Status = "Pending"
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

        suggestion.Status = updateDto.Status;
        suggestion.ReviewedBy = updateDto.ReviewedBy;
        suggestion.ReviewNotes = updateDto.ReviewNotes;

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
    public string BookTitle { get; set; } = string.Empty;
    public string? Author { get; set; }
    public string? ISBN { get; set; }
    public string? Reason { get; set; }
    public Guid SuggestedBy { get; set; }
    public Guid LibraryId { get; set; }
}

public class UpdateSuggestionStatusDto
{
    public string Status { get; set; } = string.Empty;
    public Guid? ReviewedBy { get; set; }
    public string? ReviewNotes { get; set; }
}
