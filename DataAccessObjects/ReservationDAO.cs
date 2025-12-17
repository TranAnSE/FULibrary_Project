using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public class ReservationDAO : BaseDAO<Reservation>, IReservationDAO
{
    public ReservationDAO(FULibraryDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Reservation>> GetByUserAsync(Guid userId)
    {
        return await _dbSet
            .Where(r => r.UserId == userId)
            .Include(r => r.Book)
            .ThenInclude(b => b.Category)
            .OrderByDescending(r => r.ReservationDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetActiveByUserAsync(Guid userId)
    {
        return await _dbSet
            .Where(r => r.UserId == userId && r.Status == ReservationStatus.Pending)
            .Include(r => r.Book)
            .OrderBy(r => r.ExpiryDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetPendingAsync()
    {
        return await _dbSet
            .Where(r => r.Status == ReservationStatus.Pending)
            .Include(r => r.User)
            .Include(r => r.Book)
            .OrderBy(r => r.ReservationDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetExpiredAsync()
    {
        var today = DateTime.UtcNow.Date;
        return await _dbSet
            .Where(r => r.Status == ReservationStatus.Pending && r.ExpiryDate < today)
            .Include(r => r.User)
            .Include(r => r.Book)
            .ToListAsync();
    }

    public async Task<Reservation?> GetWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(r => r.User)
            .Include(r => r.Book)
            .ThenInclude(b => b.Category)
            .Include(r => r.Book)
            .ThenInclude(b => b.Copies)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
}
