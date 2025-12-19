using AutoMapper;
using LibraryManagementAPI.DTOs.Loans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Services;

namespace LibraryManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Allow any authenticated user
public class LoansController : ODataController
{
    private readonly ILoanService _loanService;
    private readonly IMapper _mapper;

    public LoansController(ILoanService loanService, IMapper mapper)
    {
        _loanService = loanService;
        _mapper = mapper;
    }

    [HttpGet]
    [EnableQuery]
    [Authorize(Policy = "Librarian")] // Only librarians can query all loans
    public async Task<IActionResult> Get([FromQuery] Guid? userId, [FromQuery] Guid? libraryId)
    {
        // Get libraryId from middleware context (for Librarian scope)
        var scopedLibraryId = HttpContext.Items["LibraryId"] as Guid?;
        var filterLibraryId = scopedLibraryId ?? libraryId;

        IEnumerable<BusinessObjects.Loan> loans;

        if (userId.HasValue)
        {
            loans = await _loanService.GetByUserAsync(userId.Value);
        }
        else if (filterLibraryId.HasValue)
        {
            loans = await _loanService.GetByLibraryAsync(filterLibraryId.Value);
        }
        else
        {
            return BadRequest(new { message = "Either userId or libraryId is required. Librarians should have library scope automatically applied." });
        }

        var loanDtos = _mapper.Map<List<LoanDto>>(loans);
        return Ok(loanDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var loan = await _loanService.GetByIdAsync(id);
        if (loan == null)
            return NotFound();

        var loanDto = _mapper.Map<LoanDto>(loan);
        return Ok(loanDto);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(Guid userId)
    {
        var loans = await _loanService.GetByUserAsync(userId);
        var loanDtos = _mapper.Map<List<LoanDto>>(loans);
        return Ok(loanDtos);
    }

    [HttpGet("user/{userId}/active")]
    public async Task<IActionResult> GetActiveByUser(Guid userId)
    {
        var loans = await _loanService.GetActiveByUserAsync(userId);
        var loanDtos = _mapper.Map<List<LoanDto>>(loans);
        return Ok(loanDtos);
    }

    [HttpGet("overdue")]
    [Authorize(Policy = "Librarian")]
    public async Task<IActionResult> GetOverdue([FromQuery] Guid? libraryId)
    {
        // Get libraryId from middleware context (for Librarian scope)
        var scopedLibraryId = HttpContext.Items["LibraryId"] as Guid?;
        var filterLibraryId = scopedLibraryId ?? libraryId;

        var loans = await _loanService.GetOverdueLoansAsync();

        // Apply library filter if applicable
        if (filterLibraryId.HasValue)
        {
            loans = loans.Where(l => l.LibraryId == filterLibraryId.Value);
        }

        var loanDtos = _mapper.Map<List<LoanDto>>(loans);
        return Ok(loanDtos);
    }

    [HttpGet("due-soon")]
    [Authorize(Policy = "Librarian")]
    public async Task<IActionResult> GetDueSoon([FromQuery] int days = 7, [FromQuery] Guid? libraryId = null)
    {
        // Get libraryId from middleware context (for Librarian scope)
        var scopedLibraryId = HttpContext.Items["LibraryId"] as Guid?;
        var filterLibraryId = scopedLibraryId ?? libraryId;

        var loans = await _loanService.GetDueSoonAsync(days);

        // Apply library filter if applicable
        if (filterLibraryId.HasValue)
        {
            loans = loans.Where(l => l.LibraryId == filterLibraryId.Value);
        }

        var loanDtos = _mapper.Map<List<LoanDto>>(loans);
        return Ok(loanDtos);
    }

    [HttpPost]
    [Authorize(Policy = "Librarian")]
    public async Task<IActionResult> Create([FromBody] CreateLoanDto createLoanDto)
    {
        try
        {
            // Get libraryId from middleware context (for Librarian scope)
            // If not set by middleware, use the one from DTO (for flexibility)
            var scopedLibraryId = HttpContext.Items["LibraryId"] as Guid?;
            var libraryId = scopedLibraryId.HasValue && scopedLibraryId.Value != Guid.Empty
                ? scopedLibraryId.Value
                : createLoanDto.LibraryId;

            if (libraryId == Guid.Empty)
            {
                return BadRequest(new { message = "Library ID is required for loan creation" });
            }

            var loan = await _loanService.CreateLoanAsync(
                createLoanDto.UserId,
                createLoanDto.BookCopyId,
                libraryId);

            var loanDto = _mapper.Map<LoanDto>(loan);
            return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loanDto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/return")]
    [Authorize(Policy = "Librarian")]
    public async Task<IActionResult> ReturnBook(Guid id)
    {
        var result = await _loanService.ReturnBookAsync(id);
        if (!result)
            return NotFound();

        return Ok(new { message = "Book returned successfully" });
    }

    [HttpPost("{id}/renew")]
    public async Task<IActionResult> Renew(Guid id)
    {
        var result = await _loanService.RenewLoanAsync(id);
        if (!result)
            return BadRequest(new { message = "Renewal failed. Loan may be overdue or at max renewals" });

        return Ok(new { message = "Loan renewed successfully" });
    }
}
