using BusinessObjects;

namespace Services;

public interface IReservationService
{
    Task<Reservation?> GetByIdAsync(Guid id);
    Task<IEnumerable<Reservation>> GetByUserAsync(Guid userId);
    Task<IEnumerable<Reservation>> GetActiveByUserAsync(Guid userId);
    Task<Reservation> CreateReservationAsync(Guid userId, Guid bookId);
    Task<bool> CancelReservationAsync(Guid reservationId);
    Task<bool> FulfillReservationAsync(Guid reservationId);
    Task<IEnumerable<Reservation>> GetPendingAsync();
    Task<bool> ExpireReservationsAsync();
}
