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
        var reservations = await _apiService.GetAsync<List<dynamic>>($"api/reservations/user/{userId}/active") ?? new List<dynamic>();
        return View(reservations);
    }

    public async Task<IActionResult> History()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var reservations = await _apiService.GetAsync<List<dynamic>>($"api/reservations/user/{userId}") ?? new List<dynamic>();
        return View(reservations);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var result = await _apiService.PostAsync<dynamic>($"api/reservations/{id}/cancel");
        TempData[result != null ? "Success" : "Error"] = result != null ? "Reservation cancelled successfully." : "Failed to cancel reservation.";
        return RedirectToAction(nameof(Current));
    }
}
