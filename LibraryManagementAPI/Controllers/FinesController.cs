using AutoMapper;
using LibraryManagementAPI.DTOs.Fines;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace LibraryManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "Librarian")]
public class FinesController : ControllerBase
{
    private readonly IFineService _fineService;
    private readonly IMapper _mapper;

    public FinesController(IFineService fineService, IMapper mapper)
    {
        _fineService = fineService;
        _mapper = mapper;
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(Guid userId)
    {
        var fines = await _fineService.GetByUserAsync(userId);
        var fineDtos = _mapper.Map<List<FineDto>>(fines);
        return Ok(fineDtos);
    }

    [HttpGet("user/{userId}/pending")]
    public async Task<IActionResult> GetPendingByUser(Guid userId)
    {
        var fines = await _fineService.GetPendingByUserAsync(userId);
        var fineDtos = _mapper.Map<List<FineDto>>(fines);
        return Ok(fineDtos);
    }

    [HttpGet("user/{userId}/total")]
    public async Task<IActionResult> GetTotalPending(Guid userId)
    {
        var total = await _fineService.GetTotalPendingAmountByUserAsync(userId);
        return Ok(new { userId, totalPending = total });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var fine = await _fineService.GetByIdAsync(id);
        if (fine == null)
            return NotFound();

        var fineDto = _mapper.Map<FineDto>(fine);
        return Ok(fineDto);
    }

    [HttpPost("overdue")]
    public async Task<IActionResult> CreateOverdueFine([FromBody] CreateOverdueFineDto createDto)
    {
        var fine = await _fineService.CreateOverdueFineAsync(createDto.LoanId, createDto.OverdueDays);
        var fineDto = _mapper.Map<FineDto>(fine);
        return CreatedAtAction(nameof(GetById), new { id = fine.Id }, fineDto);
    }

    [HttpPost("payment")]
    public async Task<IActionResult> RecordPayment([FromBody] CreateFinePaymentDto paymentDto)
    {
        var result = await _fineService.RecordPaymentAsync(
            paymentDto.FineId, 
            paymentDto.Amount, 
            paymentDto.ReceivedBy);
        
        if (!result)
            return NotFound();

        return Ok(new { message = "Payment recorded successfully" });
    }

    [HttpPost("waive")]
    public async Task<IActionResult> WaiveFine([FromBody] WaiverDto waiverDto)
    {
        var result = await _fineService.WaiveFineAsync(
            waiverDto.FineId, 
            waiverDto.Reason, 
            waiverDto.WaivedBy);
        
        if (!result)
            return NotFound();

        return Ok(new { message = "Fine waived successfully" });
    }

    [HttpPost("reduce")]
    public async Task<IActionResult> ReduceFine([FromBody] ReduceFineDto reduceDto)
    {
        var result = await _fineService.ReduceFineAsync(
            reduceDto.FineId, 
            reduceDto.NewAmount, 
            reduceDto.Reason);
        
        if (!result)
            return BadRequest(new { message = "Fine reduction failed" });

        return Ok(new { message = "Fine reduced successfully" });
    }
}

public class CreateOverdueFineDto
{
    public Guid LoanId { get; set; }
    public int OverdueDays { get; set; }
}
