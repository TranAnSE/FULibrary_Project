using AutoMapper;
using LibraryManagementAPI.DTOs.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace LibraryManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "Librarian")]
public class BorrowersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public BorrowersController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    /// <summary>
    /// Get all borrowers. Librarians see only borrowers from their assigned library.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? libraryId = null)
    {
        // Get libraryId from middleware context (for Librarian scope)
        var scopedLibraryId = HttpContext.Items["LibraryId"] as Guid?;
        var filterLibraryId = scopedLibraryId ?? libraryId;

        var users = await _userService.GetAllAsync();
        
        // Filter only Borrowers
        var borrowers = users.Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Borrower"));
        
        // Apply library filter if applicable
        if (filterLibraryId.HasValue)
        {
            borrowers = borrowers.Where(u => u.HomeLibraryId == filterLibraryId.Value);
        }

        var borrowerDtos = _mapper.Map<List<UserDto>>(borrowers);
        return Ok(borrowerDtos);
    }

    /// <summary>
    /// Get borrower by ID. Librarians can only access borrowers from their library.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _userService.GetWithRolesAsync(id);
        if (user == null)
            return NotFound();

        // Verify user is a Borrower
        if (!user.UserRoles.Any(ur => ur.Role.Name == "Borrower"))
            return NotFound(new { message = "User is not a Borrower" });

        // Check library scope for Librarian
        var scopedLibraryId = HttpContext.Items["LibraryId"] as Guid?;
        if (scopedLibraryId.HasValue && user.HomeLibraryId != scopedLibraryId.Value)
        {
            return Forbid();
        }

        var userDto = _mapper.Map<UserDto>(user);
        return Ok(userDto);
    }

    /// <summary>
    /// Update borrower information (NOT for creating new borrowers).
    /// Librarians can only update borrowers from their assigned library.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto updateUserDto)
    {
        if (id != updateUserDto.Id)
            return BadRequest();

        var existingUser = await _userService.GetWithRolesAsync(id);
        if (existingUser == null)
            return NotFound();

        // Verify user is a Borrower
        if (!existingUser.UserRoles.Any(ur => ur.Role.Name == "Borrower"))
            return BadRequest(new { message = "Can only update Borrower accounts" });

        // Check library scope for Librarian
        var scopedLibraryId = HttpContext.Items["LibraryId"] as Guid?;
        if (scopedLibraryId.HasValue && existingUser.HomeLibraryId != scopedLibraryId.Value)
        {
            return Forbid();
        }

        _mapper.Map(updateUserDto, existingUser);
        var updatedUser = await _userService.UpdateUserAsync(existingUser);
        
        var userDto = _mapper.Map<UserDto>(updatedUser);
        return Ok(userDto);
    }

    /// <summary>
    /// Lock a borrower account (prevent borrowing).
    /// Librarians can only lock borrowers from their assigned library.
    /// </summary>
    [HttpPost("{id}/lock")]
    public async Task<IActionResult> Lock(Guid id, [FromBody] LockUserDto? lockDto = null)
    {
        var user = await _userService.GetWithRolesAsync(id);
        if (user == null)
            return NotFound();

        // Verify user is a Borrower
        if (!user.UserRoles.Any(ur => ur.Role.Name == "Borrower"))
            return BadRequest(new { message = "Can only lock Borrower accounts" });

        // Check library scope for Librarian
        var scopedLibraryId = HttpContext.Items["LibraryId"] as Guid?;
        if (scopedLibraryId.HasValue && user.HomeLibraryId != scopedLibraryId.Value)
        {
            return Forbid();
        }

        var result = await _userService.LockUserAsync(id);
        if (!result)
            return NotFound();

        return Ok(new { message = "Borrower locked successfully", reason = lockDto?.Reason });
    }

    /// <summary>
    /// Unlock a borrower account (restore borrowing privileges).
    /// Librarians can only unlock borrowers from their assigned library.
    /// </summary>
    [HttpPost("{id}/unlock")]
    public async Task<IActionResult> Unlock(Guid id)
    {
        var user = await _userService.GetWithRolesAsync(id);
        if (user == null)
            return NotFound();

        // Verify user is a Borrower
        if (!user.UserRoles.Any(ur => ur.Role.Name == "Borrower"))
            return BadRequest(new { message = "Can only unlock Borrower accounts" });

        // Check library scope for Librarian
        var scopedLibraryId = HttpContext.Items["LibraryId"] as Guid?;
        if (scopedLibraryId.HasValue && user.HomeLibraryId != scopedLibraryId.Value)
        {
            return Forbid();
        }

        var result = await _userService.UnlockUserAsync(id);
        if (!result)
            return NotFound();

        return Ok(new { message = "Borrower unlocked successfully" });
    }

    /// <summary>
    /// Search borrowers by name, email, or card number.
    /// Librarians see only borrowers from their assigned library.
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return BadRequest(new { message = "Search term is required" });

        var users = await _userService.GetAllAsync();
        
        // Filter only Borrowers
        var borrowers = users.Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Borrower"));

        // Apply library scope
        var scopedLibraryId = HttpContext.Items["LibraryId"] as Guid?;
        if (scopedLibraryId.HasValue)
        {
            borrowers = borrowers.Where(u => u.HomeLibraryId == scopedLibraryId.Value);
        }

        // Search by term
        var searchResults = borrowers.Where(u =>
            u.FullName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
            u.Email.Contains(term, StringComparison.OrdinalIgnoreCase) ||
            (u.CardNumber != null && u.CardNumber.Contains(term, StringComparison.OrdinalIgnoreCase))
        );

        var borrowerDtos = _mapper.Map<List<UserDto>>(searchResults);
        return Ok(borrowerDtos);
    }

    /// <summary>
    /// Get borrower by card number.
    /// Librarians can only access borrowers from their library.
    /// </summary>
    [HttpGet("card/{cardNumber}")]
    public async Task<IActionResult> GetByCardNumber(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return BadRequest(new { message = "Card number is required" });

        var users = await _userService.GetAllAsync();
        
        // Filter only Borrowers and by card number
        var borrower = users.FirstOrDefault(u => 
            u.UserRoles.Any(ur => ur.Role.Name == "Borrower") &&
            u.CardNumber == cardNumber);

        if (borrower == null)
            return NotFound();

        // Check library scope for Librarian
        var scopedLibraryId = HttpContext.Items["LibraryId"] as Guid?;
        if (scopedLibraryId.HasValue && borrower.HomeLibraryId != scopedLibraryId.Value)
        {
            return Forbid();
        }

        var borrowerDto = _mapper.Map<UserDto>(borrower);
        return Ok(borrowerDto);
    }
}

/// <summary>
/// DTO for locking user with reason
/// </summary>
public class LockUserDto
{
    public string? Reason { get; set; }
}
