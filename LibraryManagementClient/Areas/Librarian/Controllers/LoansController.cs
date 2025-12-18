using LibraryManagementClient.Models;
using LibraryManagementClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementClient.Areas.Librarian.Controllers;

[Area("Librarian")]
[Authorize(Roles = "Librarian,Admin")]
public class LoansController : Controller
{
    private readonly IApiService _apiService;

    public LoansController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        // API automatically filters by librarian's assigned library through middleware
        var loans = await _apiService.GetAsync<List<LoanDto>>("api/loans");
        return View(loans ?? new List<LoanDto>());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateLoanViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // If Card Number provided, find the borrower
        if (!string.IsNullOrEmpty(model.CardNumber))
        {
            var borrower = await _apiService.GetAsync<UserDto>($"api/borrowers/card/{model.CardNumber}");
            if (borrower != null)
            {
                model.UserId = borrower.Id;
                model.BorrowerName = borrower.FullName;
            }
            else
            {
                ModelState.AddModelError("CardNumber", "Borrower with this Card Number not found");
                return View(model);
            }
        }

        // If Registration Number provided, find the book copy
        if (!string.IsNullOrEmpty(model.RegistrationNumber))
        {
            var bookCopy = await _apiService.GetAsync<BookCopySearchDto>($"api/bookcopies/registration/{model.RegistrationNumber}");
            if (bookCopy != null)
            {
                model.BookCopyId = bookCopy.Id;
                model.BookTitle = bookCopy.BookTitle;
            }
            else
            {
                ModelState.AddModelError("RegistrationNumber", "Book copy with this Registration Number not found");
                return View(model);
            }
        }

        try
        {
            var result = await _apiService.PostAsync<LoanDto>("api/loans", new { model.UserId, model.BookCopyId });

            if (result != null)
            {
                TempData["Success"] = "Loan created successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Failed to create loan.";
            return View(model);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Failed to create loan: {ex.Message}";
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> SearchBorrower(string cardNumber)
    {
        var borrower = await _apiService.GetAsync<UserDto>($"api/borrowers/card/{cardNumber}");
        return Json(borrower);
    }

    [HttpGet]
    public async Task<IActionResult> SearchBookCopy(string registrationNumber)
    {
        var bookCopy = await _apiService.GetAsync<BookCopySearchDto>($"api/bookcopies/registration/{registrationNumber}");
        return Json(bookCopy);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Return(Guid id)
    {
        try
        {
            await _apiService.PostAsync<object>($"api/loans/{id}/return");
            TempData["Success"] = "Book returned successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Failed to return book: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Renew(Guid id)
    {
        try
        {
            await _apiService.PostAsync<object>($"api/loans/{id}/renew");
            TempData["Success"] = "Loan renewed successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Failed to renew loan: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Overdue()
    {
        var loans = await _apiService.GetAsync<List<LoanDto>>("api/loans/overdue");
        return View(loans ?? new List<LoanDto>());
    }
}
