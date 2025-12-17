using AutoMapper;
using LibraryManagementAPI.DTOs.Loans;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Services;

namespace LibraryManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
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
    public async Task<IActionResult> Get([FromQuery] Guid? userId, [FromQuery] Guid? libraryId)
    {
        IEnumerable<BusinessObjects.Loan> loans;

        if (userId.HasValue)
            loans = await _loanService.GetByUserAsync(userId.Value);
        else if (libraryId.HasValue)
            loans = await _loanService.GetByLibraryAsync(libraryId.Value);
        else
            return BadRequest("Either userId or libraryId is required");

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

    [HttpGet("user/{userId}/active")]
    public async Task<IActionResult> GetActiveByUser(Guid userId)
    {
        var loans = await _loanService.GetActiveByUserAsync(userId);
        var loanDtos = _mapper.Map<List<LoanDto>>(loans);
        return Ok(loanDtos);
    }

    [HttpGet("overdue")]
    public async Task<IActionResult> GetOverdue()
    {
        var loans = await _loanService.GetOverdueLoansAsync();
        var loanDtos = _mapper.Map<List<LoanDto>>(loans);
        return Ok(loanDtos);
    }

    [HttpGet("due-soon")]
    public async Task<IActionResult> GetDueSoon([FromQuery] int days = 7)
    {
        var loans = await _loanService.GetDueSoonAsync(days);
        var loanDtos = _mapper.Map<List<LoanDto>>(loans);
        return Ok(loanDtos);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLoanDto createLoanDto)
    {
        try
        {
            var loan = await _loanService.CreateLoanAsync(
                createLoanDto.UserId, 
                createLoanDto.BookCopyId, 
                createLoanDto.LibraryId);
            
            var loanDto = _mapper.Map<LoanDto>(loan);
            return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loanDto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/return")]
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
