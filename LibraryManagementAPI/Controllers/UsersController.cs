using AutoMapper;
using LibraryManagementAPI.DTOs.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Services;
using BusinessObjects;

namespace LibraryManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "Admin")]
public class UsersController : ODataController
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UsersController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    [HttpGet]
    [EnableQuery]
    public async Task<IActionResult> Get()
    {
        var users = await _userService.GetAllAsync();
        var userDtos = _mapper.Map<List<UserDto>>(users);
        return Ok(userDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _userService.GetWithRolesAsync(id);
        if (user == null)
            return NotFound();

        var userDto = _mapper.Map<UserDto>(user);
        return Ok(userDto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto createUserDto)
    {
        var user = _mapper.Map<User>(createUserDto);
        var randomPassword = Guid.NewGuid().ToString("N").Substring(0, 12);
        
        var createdUser = await _userService.CreateUserAsync(user, randomPassword);
        
        // TODO: Generate magic link and send email
        
        var userDto = _mapper.Map<UserDto>(createdUser);
        return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, userDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto updateUserDto)
    {
        if (id != updateUserDto.Id)
            return BadRequest();

        var existingUser = await _userService.GetByIdAsync(id);
        if (existingUser == null)
            return NotFound();

        _mapper.Map(updateUserDto, existingUser);
        var updatedUser = await _userService.UpdateUserAsync(existingUser);
        
        var userDto = _mapper.Map<UserDto>(updatedUser);
        return Ok(userDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _userService.DeleteUserAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpPost("{id}/lock")]
    public async Task<IActionResult> Lock(Guid id)
    {
        var result = await _userService.LockUserAsync(id);
        if (!result)
            return NotFound();

        return Ok(new { message = "User locked successfully" });
    }

    [HttpPost("{id}/unlock")]
    public async Task<IActionResult> Unlock(Guid id)
    {
        var result = await _userService.UnlockUserAsync(id);
        if (!result)
            return NotFound();

        return Ok(new { message = "User unlocked successfully" });
    }
}
