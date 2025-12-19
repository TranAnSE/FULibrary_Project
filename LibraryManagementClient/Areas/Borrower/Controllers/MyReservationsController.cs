using LibraryManagementClient.Models;
using LibraryManagementClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagementClient.Areas.Borrower.Controllers;

[Area("Borrower")]
[Authorize(Roles = "Borrower,Librarian,Admin")]
public class MyReservationsController : Controller
{
    private readonly IApiService _apiService;

    public MyReservationsController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Current()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var reservations = await _apiService.GetAsync<List<ReservationDto>>($"api/reservations/user/{userId}/active") ?? new List<ReservationDto>();
        return View(reservations);
    }

    public async Task<IActionResult> History()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var reservations = await _apiService.GetAsync<List<ReservationDto>>($"api/reservations/user/{userId}") ?? new List<ReservationDto>();
        return View(reservations);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id)
    {
        try
        {
            var result = await _apiService.PostAsync<object>($"api/reservations/{id}/cancel");
            TempData["Success"] = "Reservation cancelled successfully.";
            return RedirectToAction(nameof(Current));
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Failed to cancel reservation.";
            return RedirectToAction(nameof(Current));
        }
    }
}
