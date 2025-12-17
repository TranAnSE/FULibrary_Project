using BusinessObjects;
using DataAccessObjects;

namespace Repositories;

public class ReservationRepository : Repository<Reservation>, IReservationRepository
{
    private readonly IReservationDAO _reservationDAO;

    public ReservationRepository(IReservationDAO reservationDAO) : base(reservationDAO)
    {
        _reservationDAO = reservationDAO;
    }

    public async Task<IEnumerable<Reservation>> GetByUserAsync(Guid userId)
    {
        return await _reservationDAO.GetByUserAsync(userId);
    }

    public async Task<IEnumerable<Reservation>> GetActiveByUserAsync(Guid userId)
    {
        return await _reservationDAO.GetActiveByUserAsync(userId);
    }

    public async Task<IEnumerable<Reservation>> GetPendingAsync()
    {
        return await _reservationDAO.GetPendingAsync();
    }

    public async Task<IEnumerable<Reservation>> GetExpiredAsync()
    {
        return await _reservationDAO.GetExpiredAsync();
    }

    public async Task<Reservation?> GetWithDetailsAsync(Guid id)
    {
        return await _reservationDAO.GetWithDetailsAsync(id);
    }
}
