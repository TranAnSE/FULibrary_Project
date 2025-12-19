using LibraryManagementClient.Models;
using LibraryManagementClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementClient.Areas.Librarian.Controllers;

[Area("Librarian")]
[Authorize(Roles = "Librarian,Admin")]
public class ReservationsController : Controller
{
    private readonly IApiService _apiService;

    public ReservationsController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        var reservations = await _apiService.GetAsync<List<ReservationDto>>("api/reservations/pending") ?? new List<ReservationDto>();
        return View(reservations);
    }

    public async Task<IActionResult> Pending()
    {
        var reservations = await _apiService.GetAsync<List<ReservationDto>>("api/reservations/pending") ?? new List<ReservationDto>();
        return View(reservations);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Fulfill(Guid id)
    {
        try
        {
            var result = await _apiService.PostAsync<object>($"api/reservations/{id}/fulfill");
            TempData["Success"] = "Reservation fulfilled successfully.";
            return RedirectToAction(nameof(Pending));
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Failed to fulfill reservation.";
            return RedirectToAction(nameof(Pending));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id)
    {
        try
        {
            var result = await _apiService.PostAsync<object>($"api/reservations/{id}/cancel");
            TempData["Success"] = "Reservation cancelled successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Failed to cancel reservation.";
            return RedirectToAction(nameof(Index));
        }
    }
}
