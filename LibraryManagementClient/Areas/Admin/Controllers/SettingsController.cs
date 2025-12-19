using LibraryManagementClient.Models;
using LibraryManagementClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementClient.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class SettingsController : Controller
{
    private readonly IApiService _apiService;

    public SettingsController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        var libraries = await _apiService.GetAsync<List<LibraryDto>>("api/libraries") ?? new List<LibraryDto>();
        var settingsList = new List<SystemSettingsDto>();

        foreach (var library in libraries)
        {
            var settings = await _apiService.GetAsync<SystemSettingsDto>($"api/systemsettings/library/{library.Id}");
            if (settings != null)
            {
                // Ensure LibraryName is populated from library name if API doesn't provide it
                if (string.IsNullOrEmpty(settings.LibraryName))
                {
                    settings.LibraryName = library.Name;
                }
                settingsList.Add(settings);
            }
        }

        ViewBag.Libraries = libraries;
        return View(settingsList);
    }

    public async Task<IActionResult> Edit(Guid libraryId)
    {
        var settings = await _apiService.GetAsync<SystemSettingsDto>($"api/systemsettings/library/{libraryId}");
        
        if (settings == null)
        {
            TempData["Error"] = "Settings not found for this library.";
            return RedirectToAction(nameof(Index));
        }

        return View(settings);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SystemSettingsDto model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var updateDto = new UpdateSystemSettingsDto
        {
            Id = model.Id,
            MaxBooksPerBorrower = model.MaxBooksPerBorrower,
            LoanDurationDays = model.LoanDurationDays,
            MaxRenewals = model.MaxRenewals,
            RenewalDays = model.RenewalDays,
            DailyFineRate = model.DailyFineRate,
            LostBookFinePercent = model.LostBookFinePercent,
            ReservationExpiryDays = model.ReservationExpiryDays
        };

        var result = await _apiService.PutAsync<SystemSettingsDto>($"api/systemsettings/{model.Id}", updateDto);
        
        if (result != null)
        {
            TempData["Success"] = $"Settings for {model.LibraryName} updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Failed to update settings");
        return View(model);
    }
}
