using LibraryManagementClient.Models;
using LibraryManagementClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementClient.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly IApiService _apiService;

    public UsersController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _apiService.GetAsync<List<UserDto>>("api/users");
        return View(users ?? new List<UserDto>());
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var user = await _apiService.GetAsync<UserDto>($"api/users/{id}");
        if (user == null)
        {
            TempData["Error"] = "User not found.";
            return RedirectToAction(nameof(Index));
        }
        return View(user);
    }

    public async Task<IActionResult> Create()
    {
        await LoadLibraries();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserDto model, List<string> Roles)
    {
        if (!ModelState.IsValid)
        {
            await LoadLibraries();
            return View(model);
        }

        model.Roles = Roles ?? new List<string>();

        var result = await _apiService.PostAsync<UserDto>("api/users", model);
        if (result != null)
        {
            TempData["Success"] = "User created successfully. User must change password on first login.";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Failed to create user");
        await LoadLibraries();
        return View(model);
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var user = await _apiService.GetAsync<UserDto>($"api/users/{id}");
        if (user == null)
        {
            TempData["Error"] = "User not found.";
            return RedirectToAction(nameof(Index));
        }

        var model = new UpdateUserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            CardNumber = user.CardNumber,
            PhoneNumber = user.PhoneNumber,
            HomeLibraryId = user.HomeLibraryId,
            AssignedLibraryId = user.AssignedLibraryId,
            Roles = user.Roles
        };

        ViewData["UserRoles"] = user.Roles;
        await LoadLibraries();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateUserDto model, List<string> Roles)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            ViewData["UserRoles"] = Roles ?? new List<string>();
            await LoadLibraries();
            return View(model);
        }

        model.Roles = Roles ?? new List<string>();

        var result = await _apiService.PutAsync<UserDto>($"api/users/{id}", model);
        if (result != null)
        {
            TempData["Success"] = "User updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Failed to update user");
        ViewData["UserRoles"] = Roles ?? new List<string>();
        await LoadLibraries();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _apiService.DeleteAsync($"api/users/{id}");
        if (result)
        {
            TempData["Success"] = "User deleted successfully.";
        }
        else
        {
            TempData["Error"] = "Failed to delete user.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Lock(Guid id)
    {
        var result = await _apiService.PostAsync<ApiResponse>($"api/usermanagement/users/{id}/lock");
        if (result != null)
        {
            TempData["Success"] = "User locked successfully.";
        }
        else
        {
            TempData["Error"] = "Failed to lock user.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unlock(Guid id)
    {
        var result = await _apiService.PostAsync<ApiResponse>($"api/usermanagement/users/{id}/unlock");
        if (result != null)
        {
            TempData["Success"] = "User unlocked successfully.";
        }
        else
        {
            TempData["Error"] = "Failed to unlock user.";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadLibraries()
    {
        var libraries = await _apiService.GetAsync<List<LibraryDto>>("api/libraries");
        ViewBag.Libraries = libraries ?? new List<LibraryDto>();
    }
}
