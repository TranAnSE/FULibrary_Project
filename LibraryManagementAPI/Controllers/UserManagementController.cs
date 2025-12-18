using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace LibraryManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "Admin")]
public class UserManagementController : ControllerBase
{
    private readonly IUserService _userService;

    public UserManagementController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("users/{id}/lock")]
    public async Task<IActionResult> LockUser(Guid id)
    {
        var result = await _userService.LockUserAsync(id);
        if (!result)
            return NotFound();

        return Ok(new { message = "User locked successfully" });
    }

    [HttpPost("users/{id}/unlock")]
    public async Task<IActionResult> UnlockUser(Guid id)
    {
        var result = await _userService.UnlockUserAsync(id);
        if (!result)
            return NotFound();

        return Ok(new { message = "User unlocked successfully" });
    }
}
