using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public class MagicLinkTokenDAO : BaseDAO<MagicLinkToken>, IMagicLinkTokenDAO
{
    public MagicLinkTokenDAO(FULibraryDbContext context) : base(context)
    {
    }

    public async Task<MagicLinkToken?> GetByTokenAsync(string token)
    {
        return await _dbSet
            .Include(mlt => mlt.User)
            .FirstOrDefaultAsync(mlt => mlt.Token == token);
    }

    public async Task<IEnumerable<MagicLinkToken>> GetByUserAsync(Guid userId)
    {
        return await _dbSet
            .Where(mlt => mlt.UserId == userId)
            .OrderByDescending(mlt => mlt.CreatedAt)
            .ToListAsync();
    }
}
