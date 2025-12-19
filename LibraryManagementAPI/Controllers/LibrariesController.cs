using AutoMapper;
using BusinessObjects;
using LibraryManagementAPI.DTOs.Libraries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Services;

namespace LibraryManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "Admin")]
public class LibrariesController : ODataController
{
    private readonly ILibraryService _libraryService;
    private readonly IMapper _mapper;

    public LibrariesController(ILibraryService libraryService, IMapper mapper)
    {
        _libraryService = libraryService;
        _mapper = mapper;
    }

    [HttpGet]
    [EnableQuery]
    public async Task<IActionResult> Get()
    {
        var libraries = await _libraryService.GetAllAsync();
        var libraryDtos = _mapper.Map<List<LibraryDto>>(libraries);
        return Ok(libraryDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var library = await _libraryService.GetByIdAsync(id);
        if (library == null)
            return NotFound();

        var libraryDto = _mapper.Map<LibraryDto>(library);
        return Ok(libraryDto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLibraryDto createLibraryDto)
    {
        var library = _mapper.Map<Library>(createLibraryDto);
        var createdLibrary = await _libraryService.CreateAsync(library);
        
        var libraryDto = _mapper.Map<LibraryDto>(createdLibrary);
        return CreatedAtAction(nameof(GetById), new { id = createdLibrary.Id }, libraryDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLibraryDto updateLibraryDto)
    {
        var existingLibrary = await _libraryService.GetByIdAsync(id);
        if (existingLibrary == null)
            return NotFound();

        _mapper.Map(updateLibraryDto, existingLibrary);
        var updatedLibrary = await _libraryService.UpdateAsync(existingLibrary);
        
        var libraryDto = _mapper.Map<LibraryDto>(updatedLibrary);
        return Ok(libraryDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _libraryService.DeleteAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
