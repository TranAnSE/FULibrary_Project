using AutoMapper;
using LibraryManagementAPI.DTOs.Reservations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace LibraryManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;
    private readonly IMapper _mapper;

    public ReservationsController(IReservationService reservationService, IMapper mapper)
    {
        _reservationService = reservationService;
        _mapper = mapper;
    }

    [HttpGet("user/{userId}")]
    [Authorize(Policy = "Borrower")]
    public async Task<IActionResult> GetByUser(Guid userId)
    {
        var reservations = await _reservationService.GetByUserAsync(userId);
        var reservationDtos = _mapper.Map<List<ReservationDto>>(reservations);
        return Ok(reservationDtos);
    }

    [HttpGet("user/{userId}/active")]
    [Authorize(Policy = "Borrower")]
    public async Task<IActionResult> GetActiveByUser(Guid userId)
    {
        var reservations = await _reservationService.GetActiveByUserAsync(userId);
        var reservationDtos = _mapper.Map<List<ReservationDto>>(reservations);
        return Ok(reservationDtos);
    }

    [HttpGet("pending")]
    [Authorize(Policy = "Librarian")]
    public async Task<IActionResult> GetPending()
    {
        var reservations = await _reservationService.GetPendingAsync();
        var reservationDtos = _mapper.Map<List<ReservationDto>>(reservations);
        return Ok(reservationDtos);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "Borrower")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var reservation = await _reservationService.GetByIdAsync(id);
        if (reservation == null)
            return NotFound();

        var reservationDto = _mapper.Map<ReservationDto>(reservation);
        return Ok(reservationDto);
    }

    [HttpPost]
    [Authorize(Policy = "Borrower")]
    public async Task<IActionResult> Create([FromBody] CreateReservationDto createDto)
    {
        try
        {
            var reservation = await _reservationService.CreateReservationAsync(createDto.UserId, createDto.BookId);
            var reservationDto = _mapper.Map<ReservationDto>(reservation);
            return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservationDto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/cancel")]
    [Authorize(Policy = "Borrower")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var result = await _reservationService.CancelReservationAsync(id);
        if (!result)
            return NotFound();

        return Ok(new { message = "Reservation cancelled successfully" });
    }

    [HttpPost("{id}/fulfill")]
    [Authorize(Policy = "Librarian")]
    public async Task<IActionResult> Fulfill(Guid id)
    {
        try
        {
            var result = await _reservationService.FulfillReservationAsync(id);
            if (!result)
                return NotFound();

            return Ok(new { message = "Reservation fulfilled successfully - Loan created for borrower" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("expire")]
    [Authorize(Policy = "Librarian")]
    public async Task<IActionResult> ExpireReservations()
    {
        await _reservationService.ExpireReservationsAsync();
        return Ok(new { message = "Expired reservations processed" });
    }
}
