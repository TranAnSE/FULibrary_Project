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
public class UsersController : ControllerBase
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
    public IActionResult Get()
    {
        var users = _userService.GetAllAsQueryable();
        return Ok(users);
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
        
        var createdUser = await _userService.CreateUserWithRolesAsync(user, createUserDto.Password, createUserDto.Roles);
        
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
        var updatedUser = await _userService.UpdateUserWithRolesAsync(existingUser, updateUserDto.Roles);
        
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


}
