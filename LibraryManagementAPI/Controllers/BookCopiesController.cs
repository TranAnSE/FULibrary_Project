using AutoMapper;
using BusinessObjects;
using DataAccessObjects;
using LibraryManagementAPI.DTOs.Books;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "Librarian")]
public class BookCopiesController : ControllerBase
{
    private readonly IBookCopyDAO _bookCopyDAO;
    private readonly IMapper _mapper;

    public BookCopiesController(IBookCopyDAO bookCopyDAO, IMapper mapper)
    {
        _bookCopyDAO = bookCopyDAO;
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
