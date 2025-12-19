using LibraryManagementClient.Models;
using LibraryManagementClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementClient.Areas.Librarian.Controllers;

[Area("Librarian")]
[Authorize(Roles = "Librarian,Admin")]
public class FinesController : Controller
{
    private readonly IApiService _apiService;

    public FinesController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index(Guid? userId)
    {
        // For librarians, we'd need to fetch all fines or implement filter by library
        // For now, showing all pending fines
        ViewBag.UserId = userId;
        return View();
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var fine = await _apiService.GetAsync<FineDto>($"api/fines/{id}");
        
        if (fine == null)
        {
            TempData["Error"] = "Fine not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(fine);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RecordPayment(Guid fineId, decimal amount, string? notes)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var receivedBy))
        {
            TempData["Error"] = "Invalid user session.";
            return RedirectToAction(nameof(Index));
        }

        var paymentDto = new CreateFinePaymentDto
        {
            FineId = fineId,
            Amount = amount,
            Notes = notes,
            ReceivedBy = receivedBy
        };

        try
        {
            var result = await _apiService.PostAsync<dynamic>("api/fines/payment", paymentDto);
            
            if (result != null)
            {
                TempData["Success"] = "Payment recorded successfully.";
                return RedirectToAction(nameof(Details), new { id = fineId });
            }
        }
        catch (HttpRequestException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Details), new { id = fineId });
        }

        TempData["Error"] = "Failed to record payment.";
        return RedirectToAction(nameof(Details), new { id = fineId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Waive(Guid fineId, string reason)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var waivedBy))
        {
            TempData["Error"] = "Invalid user session.";
            return RedirectToAction(nameof(Index));
        }

        var waiverDto = new WaiverDto
        {
            FineId = fineId,
            Reason = reason,
            WaivedBy = waivedBy
        };

        try
        {
            var result = await _apiService.PostAsync<dynamic>("api/fines/waive", waiverDto);
            
            if (result != null)
            {
                TempData["Success"] = "Fine waived successfully.";
                return RedirectToAction(nameof(Details), new { id = fineId });
            }
        }
        catch (HttpRequestException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Details), new { id = fineId });
        }

        TempData["Error"] = "Failed to waive fine.";
        return RedirectToAction(nameof(Details), new { id = fineId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reduce(Guid fineId, decimal newAmount, string reason)
    {
        var reduceDto = new ReduceFineDto
        {
            FineId = fineId,
            NewAmount = newAmount,
            Reason = reason
        };

        try
        {
            var result = await _apiService.PostAsync<dynamic>("api/fines/reduce", reduceDto);
            
            if (result != null)
            {
                TempData["Success"] = "Fine amount reduced successfully.";
                return RedirectToAction(nameof(Details), new { id = fineId });
            }
        }
        catch (HttpRequestException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Details), new { id = fineId });
        }

        TempData["Error"] = "Failed to reduce fine amount.";
        return RedirectToAction(nameof(Details), new { id = fineId });
    }
}
