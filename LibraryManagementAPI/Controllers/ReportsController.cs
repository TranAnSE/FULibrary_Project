using LibraryManagementAPI.DTOs.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace LibraryManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("admin/dashboard")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> GetAdminDashboard()
    {
        var totalBooksByLibrary = await _reportService.GetTotalBooksByLibraryAsync();
        var topBooks = await _reportService.GetTopBorrowedBooksByLibraryAsync();
        var availabilityRatio = await _reportService.GetAvailabilityRatioByLibraryAsync();

        var dashboard = new AdminDashboardDto
        {
            LibraryStats = totalBooksByLibrary.ToDictionary(
                kvp => kvp.Key,
                kvp => new LibraryStatsDto
                {
                    LibraryId = kvp.Key,
                    LibraryName = string.Empty,
                    TotalBooks = kvp.Value,
                    AvailableCopies = availabilityRatio.ContainsKey(kvp.Key) ? availabilityRatio[kvp.Key].Available : 0,
                    BorrowedCopies = availabilityRatio.ContainsKey(kvp.Key) ? availabilityRatio[kvp.Key].Borrowed : 0,
                    AvailabilityRatio = availabilityRatio.ContainsKey(kvp.Key) && (availabilityRatio[kvp.Key].Available + availabilityRatio[kvp.Key].Borrowed) > 0
                        ? (decimal)availabilityRatio[kvp.Key].Available / (availabilityRatio[kvp.Key].Available + availabilityRatio[kvp.Key].Borrowed) * 100
                        : 0,
                    TotalLoans = 0
                }),
            TopBorrowedBooks = topBooks.SelectMany(kvp => kvp.Value.Select(book => new TopBookDto
            {
                BookTitle = book.BookTitle,
                LoanCount = book.LoanCount,
                LibraryId = kvp.Key,
                LibraryName = string.Empty
            })).ToList()
        };

        return Ok(dashboard);
    }

    [HttpGet("librarian/dashboard")]
    [Authorize(Policy = "Librarian")]
    public async Task<IActionResult> GetLibrarianDashboard([FromQuery] Guid libraryId, [FromQuery] int dueDays = 7)
    {
        var todayLoans = await _reportService.GetTodayLoansCountAsync(libraryId);
        var dueSoon = await _reportService.GetBooksDueSoonAsync(libraryId, dueDays);
        var overdue = await _reportService.GetOverdueLoansAsync(libraryId);

        var dashboard = new LibrarianDashboardDto
        {
            TodayLoansCount = todayLoans,
            BooksDueSoon = dueSoon.Select(b => new DueSoonBookDto
            {
                BookTitle = b.BookTitle,
                DueDate = b.DueDate,
                BorrowerName = b.BorrowerName,
                DaysUntilDue = (b.DueDate.Date - DateTime.UtcNow.Date).Days
            }).ToList(),
            OverdueBooks = overdue.Select(b => new OverdueBookDto
            {
                BookTitle = b.BookTitle,
                OverdueDays = b.OverdueDays,
                BorrowerName = b.BorrowerName
            }).ToList()
        };

        return Ok(dashboard);
    }

    [HttpGet("library/{libraryId}/books/total")]
    [Authorize(Policy = "Librarian")]
    public async Task<IActionResult> GetTotalBooks(Guid libraryId)
    {
        var totalBooks = await _reportService.GetTotalBooksByLibraryAsync();
        if (!totalBooks.ContainsKey(libraryId))
            return NotFound();

        return Ok(new { libraryId, total = totalBooks[libraryId] });
    }

    [HttpGet("library/{libraryId}/books/top")]
    [Authorize(Policy = "Librarian")]
    public async Task<IActionResult> GetTopBooks(Guid libraryId, [FromQuery] int count = 5)
    {
        var topBooks = await _reportService.GetTopBorrowedBooksByLibraryAsync(count);
        if (!topBooks.ContainsKey(libraryId))
            return Ok(new List<TopBookDto>());

        var result = topBooks[libraryId].Select(b => new TopBookDto
        {
            BookTitle = b.BookTitle,
            LoanCount = b.LoanCount,
            LibraryId = libraryId,
            LibraryName = string.Empty
        });

        return Ok(result);
    }
}
