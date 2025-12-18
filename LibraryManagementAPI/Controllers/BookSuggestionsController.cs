using BusinessObjects;
using DataAccessObjects;
using LibraryManagementAPI.DTOs.BookSuggestions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "Borrower")]
public class BookSuggestionsController : ControllerBase
{
    private readonly IBookPurchaseSuggestionDAO _suggestionDAO;
    private readonly IUserDAO _userDAO;

    public BookSuggestionsController(IBookPurchaseSuggestionDAO suggestionDAO, IUserDAO userDAO)
    {
        _suggestionDAO = suggestionDAO;
        _userDAO = userDAO;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? status)
    {
        var suggestions = await _suggestionDAO.GetAllAsync();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<SuggestionStatus>(status, true, out var statusEnum))
        {
            suggestions = suggestions.Where(s => s.Status == statusEnum).ToList();
        }

        var responseDtos = suggestions.Select(s => new BookSuggestionDto
        {
            Id = s.Id,
            Title = s.Title,
            Author = s.Author,
            ISBN = s.ISBN,
            Reason = s.Reason,
            Status = s.Status,
            AdminNotes = s.AdminNotes,
            UserId = s.UserId,
            UserName = s.User?.FullName ?? string.Empty,
            LibraryId = s.LibraryId,
            LibraryName = s.Library?.Name ?? string.Empty,
            CreatedAt = s.CreatedAt
        }).ToList();

        return Ok(responseDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var suggestion = await _suggestionDAO.GetByIdAsync(id);
        if (suggestion == null)
            return NotFound();

        var responseDto = new BookSuggestionDto
        {
            Id = suggestion.Id,
            Title = suggestion.Title,
            Author = suggestion.Author,
            ISBN = suggestion.ISBN,
            Reason = suggestion.Reason,
            Status = suggestion.Status,
            AdminNotes = suggestion.AdminNotes,
            UserId = suggestion.UserId,
            UserName = suggestion.User?.FullName ?? string.Empty,
            LibraryId = suggestion.LibraryId,
            LibraryName = suggestion.Library?.Name ?? string.Empty,
            CreatedAt = suggestion.CreatedAt
        };

        return Ok(responseDto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookSuggestionDto createDto)
    {
        // Get the user's home library ID
        var libraryId = await GetUserHomeLibraryId(createDto.UserId);
        if (libraryId == null || libraryId == Guid.Empty)
        {
            return BadRequest(new { message = "User home library not found. Please ensure the user has a valid home library assigned." });
        }

        var suggestion = new BookPurchaseSuggestion
        {
            Title = createDto.Title,
            Author = createDto.Author,
            ISBN = createDto.ISBN,
            Reason = createDto.Reason,
            UserId = createDto.UserId,
            LibraryId = libraryId.Value,
            Status = SuggestionStatus.Pending
        };

        var created = await _suggestionDAO.AddAsync(suggestion);

        var responseDto = new BookSuggestionDto
        {
            Id = created.Id,
            Title = created.Title,
            Author = created.Author,
            ISBN = created.ISBN,
            Reason = created.Reason,
            Status = created.Status,
            AdminNotes = created.AdminNotes,
            UserId = created.UserId,
            LibraryId = created.LibraryId,
            CreatedAt = created.CreatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, responseDto);
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

        var responseDto = new BookSuggestionDto
        {
            Id = suggestion.Id,
            Title = suggestion.Title,
            Author = suggestion.Author,
            ISBN = suggestion.ISBN,
            Reason = suggestion.Reason,
            Status = suggestion.Status,
            AdminNotes = suggestion.AdminNotes,
            UserId = suggestion.UserId,
            UserName = suggestion.User?.FullName ?? string.Empty,
            LibraryId = suggestion.LibraryId,
            LibraryName = suggestion.Library?.Name ?? string.Empty,
            CreatedAt = suggestion.CreatedAt
        };

        return Ok(responseDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _suggestionDAO.SoftDeleteAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }

    private async Task<Guid?> GetUserHomeLibraryId(Guid userId)
    {
        var user = await _userDAO.GetByIdAsync(userId);
        return user?.HomeLibraryId;
    }
}
