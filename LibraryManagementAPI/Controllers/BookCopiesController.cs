using AutoMapper;
using BusinessObjects;
using DataAccessObjects;
using LibraryManagementAPI.DTOs.Books;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services;

namespace LibraryManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "Librarian")]
public class BookCopiesController : ControllerBase
{
    private readonly IBookCopyDAO _bookCopyDAO;
    private readonly IBookDAO _bookDAO;
    private readonly FULibraryDbContext _context;
    private readonly IMapper _mapper;

    public BookCopiesController(IBookCopyDAO bookCopyDAO, IBookDAO bookDAO, FULibraryDbContext context, IMapper mapper)
    {
        _bookCopyDAO = bookCopyDAO;
        _bookDAO = bookDAO;
        _context = context;
        _mapper = mapper;
    }

    [HttpGet("book/{bookId}")]
    public async Task<IActionResult> GetByBook(Guid bookId)
    {
        var copies = await _bookCopyDAO.GetByBookAsync(bookId);
        var copyDtos = _mapper.Map<List<BookCopyDto>>(copies);
        return Ok(copyDtos);
    }

    [HttpGet("book/{bookId}/available")]
    public async Task<IActionResult> GetAvailableCopies(Guid bookId)
    {
        var copies = await _bookCopyDAO.GetAvailableCopiesAsync(bookId);
        var copyDtos = _mapper.Map<List<BookCopyDto>>(copies);
        return Ok(copyDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var copy = await _bookCopyDAO.GetByIdAsync(id);
        if (copy == null)
            return NotFound();

        var copyDto = _mapper.Map<BookCopyDto>(copy);
        return Ok(copyDto);
    }

    /// Get book copy by registration number.
    /// Librarians can only access copies from their library.
    [HttpGet("registration/{registrationNumber}")]
    public async Task<IActionResult> GetByRegistrationNumber(string registrationNumber)
    {
        if (string.IsNullOrWhiteSpace(registrationNumber))
            return BadRequest(new { message = "Registration number is required" });

        // Get libraryId from middleware context (for Librarian scope)
        var scopedLibraryId = HttpContext.Items["LibraryId"] as Guid?;

        var bookCopy = await _context.BookCopies
            .Include(bc => bc.Book)
            .ThenInclude(b => b.Library)
            .Where(bc => bc.RegistrationNumber == registrationNumber)
            .FirstOrDefaultAsync();

        if (bookCopy == null)
            return NotFound();

        // Check library scope for Librarian
        if (scopedLibraryId.HasValue && bookCopy.Book.LibraryId != scopedLibraryId.Value)
        {
            return Forbid();
        }

        // Return simplified object with necessary info
        var result = new
        {
            id = bookCopy.Id,
            registrationNumber = bookCopy.RegistrationNumber,
            status = bookCopy.Status.ToString(),
            bookTitle = bookCopy.Book.Title,
            bookAuthor = bookCopy.Book.Author,
            bookId = bookCopy.Book.Id
        };

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookCopyDto createDto)
    {
        var bookCopy = _mapper.Map<BookCopy>(createDto);
        var createdCopy = await _bookCopyDAO.AddAsync(bookCopy);
        
        var copyDto = _mapper.Map<BookCopyDto>(createdCopy);
        return CreatedAtAction(nameof(GetById), new { id = createdCopy.Id }, copyDto);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] BookCopyStatus status)
    {
        var copy = await _bookCopyDAO.GetByIdAsync(id);
        if (copy == null)
            return NotFound();

        copy.Status = status;
        await _bookCopyDAO.UpdateAsync(copy);
        
        return Ok(new { message = "Status updated successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _bookCopyDAO.SoftDeleteAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
