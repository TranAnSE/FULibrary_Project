using AutoMapper;
using BusinessObjects;
using LibraryManagementAPI.DTOs.Books;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Services;

namespace LibraryManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "Librarian")]
public class BooksController : ODataController
{
    private readonly IBookService _bookService;
    private readonly IMapper _mapper;

    public BooksController(IBookService bookService, IMapper mapper)
    {
        _bookService = bookService;
        _mapper = mapper;
    }

    [HttpGet]
    [EnableQuery]
    [AllowAnonymous]
    public async Task<IActionResult> Get([FromQuery] Guid? libraryId = null)
    {
        // Get libraryId from middleware context (for Librarian scope)
        var scopedLibraryId = HttpContext.Items["LibraryId"] as Guid?;
        var filterLibraryId = scopedLibraryId ?? libraryId;

        var books = await _bookService.GetAllAsync();
        
        // Apply library filter if applicable
        if (filterLibraryId.HasValue)
        {
            books = books.Where(b => b.LibraryId == filterLibraryId.Value);
        }
        
        var bookDtos = _mapper.Map<List<BookDto>>(books);
        return Ok(bookDtos);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var book = await _bookService.GetWithCopiesAsync(id);
        if (book == null)
            return NotFound();

        var bookDto = _mapper.Map<BookDetailDto>(book);
        return Ok(bookDto);
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> Search([FromQuery] string term)
    {
        // Get libraryId from middleware context (for Librarian scope)
        var scopedLibraryId = HttpContext.Items["LibraryId"] as Guid?;

        var books = await _bookService.SearchAsync(term);
        
        // Apply library filter if applicable
        if (scopedLibraryId.HasValue)
        {
            books = books.Where(b => b.LibraryId == scopedLibraryId.Value);
        }
        
        var bookDtos = _mapper.Map<List<BookDto>>(books);
        return Ok(bookDtos);
    }

    [HttpGet("new")]
    [AllowAnonymous]
    public async Task<IActionResult> GetNewBooks([FromQuery] Guid? libraryId)
    {
        var books = await _bookService.GetNewBooksThisMonthAsync(libraryId);
        var bookDtos = _mapper.Map<List<BookDto>>(books);
        return Ok(bookDtos);
    }

    [HttpGet("library/{libraryId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByLibrary(Guid libraryId)
    {
        var books = await _bookService.GetByLibraryAsync(libraryId);
        var bookDtos = _mapper.Map<List<BookDto>>(books);
        return Ok(bookDtos);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookDto createBookDto)
    {
        var book = _mapper.Map<Book>(createBookDto);
        var createdBook = await _bookService.CreateAsync(book);
        
        var bookDto = _mapper.Map<BookDto>(createdBook);
        return CreatedAtAction(nameof(GetById), new { id = createdBook.Id }, bookDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBookDto updateBookDto)
    {
        if (id != updateBookDto.Id)
            return BadRequest();

        var existingBook = await _bookService.GetByIdAsync(id);
        if (existingBook == null)
            return NotFound();

        _mapper.Map(updateBookDto, existingBook);
        var updatedBook = await _bookService.UpdateAsync(existingBook);
        
        var bookDto = _mapper.Map<BookDto>(updatedBook);
        return Ok(bookDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _bookService.DeleteAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
