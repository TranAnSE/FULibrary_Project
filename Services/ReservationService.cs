using BusinessObjects;
using DataAccessObjects;
using Repositories;

namespace Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly ISystemSettingsDAO _systemSettingsDAO;
    private readonly IBookCopyDAO _bookCopyDAO;
    private readonly IUserDAO _userDAO;
    private readonly IBookDAO _bookDAO;
    private readonly ILoanDAO _loanDAO;
    private readonly ILoanRepository _loanRepository;

    public ReservationService(
        IReservationRepository reservationRepository,
        ISystemSettingsDAO systemSettingsDAO,
        IBookCopyDAO bookCopyDAO,
        IUserDAO userDAO,
        IBookDAO bookDAO,
        ILoanDAO loanDAO,
        ILoanRepository loanRepository)
    {
        _reservationRepository = reservationRepository;
        _systemSettingsDAO = systemSettingsDAO;
        _bookCopyDAO = bookCopyDAO;
        _userDAO = userDAO;
        _bookDAO = bookDAO;
        _loanDAO = loanDAO;
        _loanRepository = loanRepository;
    }

    public async Task<Reservation?> GetByIdAsync(Guid id)
    {
        return await _reservationRepository.GetWithDetailsAsync(id);
    }

    public async Task<IEnumerable<Reservation>> GetByUserAsync(Guid userId)
    {
        return await _reservationRepository.GetByUserAsync(userId);
    }

    public async Task<IEnumerable<Reservation>> GetActiveByUserAsync(Guid userId)
    {
        return await _reservationRepository.GetActiveByUserAsync(userId);
    }

    public async Task<Reservation> CreateReservationAsync(Guid userId, Guid bookId)
    {
        // 1. Check if user exists and get their home library
        var user = await _userDAO.GetByIdAsync(userId);
        if (user == null)
            throw new InvalidOperationException("User not found");

        // 2. Check if book exists and get its library
        var book = await _bookDAO.GetByIdAsync(bookId);
        if (book == null)
            throw new InvalidOperationException("Book not found");

        // 3. Check if book belongs to user's home library
        if (user.HomeLibraryId.HasValue && book.LibraryId != user.HomeLibraryId.Value)
            throw new InvalidOperationException("You can only reserve books from your home library");

        // 4. Check if all copies are borrowed (business rule: only reserve when no copies available)
        var availableCopies = await _bookCopyDAO.GetAvailableCopiesAsync(bookId);
        if (availableCopies.Any())
            throw new InvalidOperationException("This book has available copies. Please borrow it directly instead of reserving");

        // 5. Check if user already has an active reservation for this book
        var activeReservations = await _reservationRepository.GetActiveByUserAsync(userId);
        if (activeReservations.Any(r => r.BookId == bookId))
            throw new InvalidOperationException("You already have an active reservation for this book");

        // 6. Check if user currently has this book borrowed
        var activeLoans = await _loanDAO.GetActiveByUserAsync(userId);
        if (activeLoans.Any(l => l.BookCopy.BookId == bookId))
            throw new InvalidOperationException("You already have this book borrowed");

        // Create the reservation
        var expiryDays = 3;
        var reservation = new Reservation
        {
            UserId = userId,
            BookId = bookId,
            ReservationDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(expiryDays),
            Status = ReservationStatus.Pending
        };

        return await _reservationRepository.CreateAsync(reservation);
    }

    public async Task<bool> CancelReservationAsync(Guid reservationId)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation == null || reservation.Status != ReservationStatus.Pending)
            return false;

        reservation.Status = ReservationStatus.Cancelled;
        reservation.CancelledDate = DateTime.UtcNow;
        await _reservationRepository.UpdateAsync(reservation);
        return true;
    }

    public async Task<bool> FulfillReservationAsync(Guid reservationId)
    {
        // Get the reservation with details
        var reservation = await _reservationRepository.GetWithDetailsAsync(reservationId);
        if (reservation == null || reservation.Status != ReservationStatus.Pending)
            return false;

        // Check if there's an available copy of the book
        var availableCopies = await _bookCopyDAO.GetAvailableCopiesAsync(reservation.BookId);
        if (!availableCopies.Any())
            throw new InvalidOperationException("No available copies to fulfill this reservation. All copies are currently borrowed.");

        // Get user to check their home library
        var user = await _userDAO.GetByIdAsync(reservation.UserId);
        if (user == null)
            throw new InvalidOperationException("User not found");

        // Create a loan for the first available copy
        var loan = new Loan
        {
            UserId = reservation.UserId,
            BookCopyId = availableCopies.First().Id,
            LibraryId = user.HomeLibraryId ?? reservation.Book.LibraryId,
            LoanDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(14), // Default 14 days
            RenewalCount = 0,
            IsOverdue = false
        };

        // Update book copy status to borrowed
        availableCopies.First().Status = BookCopyStatus.Borrowed;
        await _bookCopyDAO.UpdateAsync(availableCopies.First());

        // Create the loan
        await _loanRepository.CreateAsync(loan);

        // Mark reservation as fulfilled
        reservation.Status = ReservationStatus.Fulfilled;
        reservation.FulfilledDate = DateTime.UtcNow;
        await _reservationRepository.UpdateAsync(reservation);

        return true;
    }

    public async Task<IEnumerable<Reservation>> GetPendingAsync()
    {
        return await _reservationRepository.GetPendingAsync();
    }

    public async Task<bool> ExpireReservationsAsync()
    {
        var expiredReservations = await _reservationRepository.GetExpiredAsync();
        foreach (var reservation in expiredReservations)
        {
            reservation.Status = ReservationStatus.Expired;
            await _reservationRepository.UpdateAsync(reservation);
        }
        return true;
    }
}
