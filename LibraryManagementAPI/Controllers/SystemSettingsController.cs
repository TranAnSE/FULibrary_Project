using AutoMapper;
using BusinessObjects;
using DataAccessObjects;
using LibraryManagementAPI.DTOs.SystemSettings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "Admin")]
public class SystemSettingsController : ControllerBase
{
    private readonly ISystemSettingsDAO _systemSettingsDAO;
    private readonly IMapper _mapper;

    public SystemSettingsController(ISystemSettingsDAO systemSettingsDAO, IMapper mapper)
    {
        _systemSettingsDAO = systemSettingsDAO;
        _mapper = mapper;
    }

    [HttpGet("library/{libraryId}")]
    public async Task<IActionResult> GetByLibrary(Guid libraryId)
    {
        var settings = await _systemSettingsDAO.GetByLibraryAsync(libraryId);
        if (settings == null)
            return NotFound();

        var settingsDto = _mapper.Map<SystemSettingsDto>(settings);
        return Ok(settingsDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSystemSettingsDto updateDto)
    {
        if (id != updateDto.Id)
            return BadRequest();

        var existingSettings = await _systemSettingsDAO.GetByIdAsync(id);
        if (existingSettings == null)
            return NotFound();

        _mapper.Map(updateDto, existingSettings);
        var updatedSettings = await _systemSettingsDAO.UpdateAsync(existingSettings);
        
        var settingsDto = _mapper.Map<SystemSettingsDto>(updatedSettings);
        return Ok(settingsDto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UpdateSystemSettingsDto createDto)
    {
        var settings = new SystemSettings
        {
            LibraryId = createDto.Id,
            MaxBooksPerBorrower = createDto.MaxBooksPerBorrower,
            LoanDurationDays = createDto.LoanDurationDays,
            MaxRenewals = createDto.MaxRenewals,
            RenewalDays = createDto.RenewalDays,
            DailyFineRate = createDto.DailyFineRate,
            LostBookFinePercent = createDto.LostBookFinePercent,
            ReservationExpiryDays = createDto.ReservationExpiryDays
        };

        var createdSettings = await _systemSettingsDAO.AddAsync(settings);
        var settingsDto = _mapper.Map<SystemSettingsDto>(createdSettings);
        
        return CreatedAtAction(nameof(GetByLibrary), new { libraryId = createdSettings.LibraryId }, settingsDto);
    }
}
