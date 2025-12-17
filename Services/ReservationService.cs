using BusinessObjects;
using DataAccessObjects;
using Repositories;

namespace Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly ISystemSettingsDAO _systemSettingsDAO;
    private readonly IBookCopyDAO _bookCopyDAO;

    public ReservationService(
        IReservationRepository reservationRepository,
        ISystemSettingsDAO systemSettingsDAO,
        IBookCopyDAO bookCopyDAO)
    {
        _reservationRepository = reservationRepository;
        _systemSettingsDAO = systemSettingsDAO;
        _bookCopyDAO = bookCopyDAO;
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
        var availableCopies = await _bookCopyDAO.GetAvailableCopiesAsync(bookId);
        if (!availableCopies.Any())
            throw new InvalidOperationException("No available copies for reservation");

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
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation == null || reservation.Status != ReservationStatus.Pending)
            return false;

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
