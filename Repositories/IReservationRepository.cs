using BusinessObjects;

namespace Repositories;

public interface IReservationRepository : IRepository<Reservation>
{
    Task<IEnumerable<Reservation>> GetByUserAsync(Guid userId);
    Task<IEnumerable<Reservation>> GetActiveByUserAsync(Guid userId);
    Task<IEnumerable<Reservation>> GetPendingAsync();
    Task<IEnumerable<Reservation>> GetExpiredAsync();
    Task<Reservation?> GetWithDetailsAsync(Guid id);
}
