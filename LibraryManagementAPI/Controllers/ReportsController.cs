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
    private readonly ILibraryService _libraryService;

    public ReportsController(IReportService reportService, ILibraryService libraryService)
    {
        _reportService = reportService;
        _libraryService = libraryService;
    }

    [HttpGet("admin/dashboard")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> GetAdminDashboard()
    {
        var totalBooksByLibrary = await _reportService.GetTotalBooksByLibraryAsync();
        var topBooks = await _reportService.GetTopBorrowedBooksByLibraryAsync();
        var availabilityRatio = await _reportService.GetAvailabilityRatioByLibraryAsync();
        var systemStats = await _reportService.GetSystemWideStatisticsAsync();
        
        // Get all libraries to map names
        var allLibraries = await _libraryService.GetAllAsync();
        var libraryNameMap = allLibraries.ToDictionary(l => l.Id, l => l.Name);

        var dashboard = new AdminDashboardDto
        {
            LibraryStats = totalBooksByLibrary.ToDictionary(
                kvp => kvp.Key,
                kvp => new LibraryStatsDto
                {
                    LibraryId = kvp.Key,
                    LibraryName = libraryNameMap.GetValueOrDefault(kvp.Key, "Unknown Library"),
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
                LibraryName = libraryNameMap.GetValueOrDefault(kvp.Key, "Unknown Library")
            })).ToList(),
            SystemStats = new SystemStatsDto
            {
                TotalLibraries = systemStats.GetValueOrDefault("Libraries", 0),
                TotalBooks = systemStats.GetValueOrDefault("TotalBooks", 0),
                ActiveUsers = systemStats.GetValueOrDefault("ActiveUsers", 0),
                ActiveLoans = systemStats.GetValueOrDefault("ActiveLoans", 0),
                PendingReservations = systemStats.GetValueOrDefault("PendingReservations", 0)
            }
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
        var pendingReservations = await _reportService.GetPendingReservationsCountAsync(libraryId);

        var dashboard = new LibrarianDashboardDto
        {
            TodayLoansCount = todayLoans,
            PendingReservationsCount = pendingReservations,
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

        // Get library name
        var library = await _libraryService.GetByIdAsync(libraryId);
        var libraryName = library?.Name ?? "Unknown Library";

        var result = topBooks[libraryId].Select(b => new TopBookDto
        {
            BookTitle = b.BookTitle,
            LoanCount = b.LoanCount,
            LibraryId = libraryId,
            LibraryName = libraryName
        });

        return Ok(result);
    }

    [HttpGet("borrower/dashboard")]
    [Authorize(Policy = "Borrower")]
    public async Task<IActionResult> GetBorrowerDashboard([FromQuery] string userId)
    {
        var activeLoans = await _reportService.GetBorrowerActiveLoansCountAsync(userId);
        var reservations = await _reportService.GetBorrowerReservationsCountAsync(userId);
        var pendingFines = await _reportService.GetBorrowerPendingFinesAsync(userId);

        var dashboard = new BorrowerDashboardDto
        {
            ActiveLoansCount = activeLoans,
            CurrentReservationsCount = reservations,
            PendingFinesCount = pendingFines.Count(),
            PendingFinesTotal = pendingFines.Sum()
        };

        return Ok(dashboard);
    }
}
