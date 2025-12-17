using BusinessObjects;

namespace DataAccessObjects;

public interface IReservationDAO : IBaseDAO<Reservation>
{
    Task<IEnumerable<Reservation>> GetByUserAsync(Guid userId);
    Task<IEnumerable<Reservation>> GetActiveByUserAsync(Guid userId);
    Task<IEnumerable<Reservation>> GetPendingAsync();
    Task<IEnumerable<Reservation>> GetExpiredAsync();
    Task<Reservation?> GetWithDetailsAsync(Guid id);
}
