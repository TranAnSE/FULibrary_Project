using BusinessObjects;
using LibraryManagementAPI.DTOs.BookSuggestions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace LibraryManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "Librarian")]
public class LibrarianBookSuggestionsController : ControllerBase
{
    private readonly IBookSuggestionRepository _suggestionRepository;

    public LibrarianBookSuggestionsController(IBookSuggestionRepository suggestionRepository)
    {
        _suggestionRepository = suggestionRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetByLibrary([FromQuery] SuggestionStatus? status = null, [FromQuery] string? searchTerm = null)
    {
        // Get libraryId from middleware context (for Librarian scope)
        var libraryId = HttpContext.Items["LibraryId"] as Guid?;
        if (!libraryId.HasValue)
        {
            return Forbid("Librarian is not assigned to any library");
        }

        var suggestions = await _suggestionRepository.GetByLibraryAsync(libraryId.Value, status, searchTerm);

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
        var suggestion = await _suggestionRepository.GetByIdAsync(id);
        if (suggestion == null)
            return NotFound();

        // Verify librarian has access to this suggestion's library
        var libraryId = HttpContext.Items["LibraryId"] as Guid?;
        if (!libraryId.HasValue || suggestion.LibraryId != libraryId.Value)
        {
            return Forbid("Librarian can only access suggestions from their assigned library");
        }

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

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] LibrarianUpdateSuggestionStatusDto updateDto)
    {
        var suggestion = await _suggestionRepository.GetByIdAsync(id);
        if (suggestion == null)
            return NotFound();

        // Verify librarian has access to this suggestion's library
        var libraryId = HttpContext.Items["LibraryId"] as Guid?;
        if (!libraryId.HasValue || suggestion.LibraryId != libraryId.Value)
        {
            return Forbid("Librarian can only update suggestions from their assigned library");
        }

        if (Enum.TryParse<SuggestionStatus>(updateDto.Status, true, out var statusEnum))
        {
            suggestion.Status = statusEnum;

            if (!string.IsNullOrEmpty(updateDto.AdminNotes))
            {
                suggestion.AdminNotes = updateDto.AdminNotes;
            }

            var updated = await _suggestionRepository.UpdateAsync(suggestion);

            var responseDto = new BookSuggestionDto
            {
                Id = updated.Id,
                Title = updated.Title,
                Author = updated.Author,
                ISBN = updated.ISBN,
                Reason = updated.Reason,
                Status = updated.Status,
                AdminNotes = updated.AdminNotes,
                UserId = updated.UserId,
                LibraryId = updated.LibraryId,
                CreatedAt = updated.CreatedAt
            };

            return Ok(responseDto);
        }

        return BadRequest("Invalid status value");
    }

    [HttpPut("bulk-status")]
    public async Task<IActionResult> BulkUpdateStatus([FromBody] BulkUpdateSuggestionStatusDto bulkUpdateDto)
    {
        var libraryId = HttpContext.Items["LibraryId"] as Guid?;
        if (!libraryId.HasValue)
        {
            return Forbid("Librarian is not assigned to any library");
        }

        if (Enum.TryParse<SuggestionStatus>(bulkUpdateDto.Status, true, out var statusEnum))
        {
            var result = await _suggestionRepository.BulkUpdateStatusAsync(
                bulkUpdateDto.SuggestionIds, 
                statusEnum, 
                Guid.NewGuid(), // This should be the librarian's ID from HttpContext - simplified for now
                bulkUpdateDto.AdminNotes);
                
            if (result)
            {
                return Ok(new { Message = "Status updated successfully", Count = bulkUpdateDto.SuggestionIds.Count() });
            }
            else
            {
                return BadRequest("Failed to update suggestions");
            }
        }

        return BadRequest("Invalid status value");
    }
}

public class LibrarianUpdateSuggestionStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string? AdminNotes { get; set; }
}

public class BulkUpdateSuggestionStatusDto
{
    public IEnumerable<Guid> SuggestionIds { get; set; } = new List<Guid>();
    public string Status { get; set; } = string.Empty;
    public string? AdminNotes { get; set; }
}
