using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public class ShelfLocationDAO : BaseDAO<ShelfLocation>, IShelfLocationDAO
{
    public ShelfLocationDAO(FULibraryDbContext context) : base(context)
    {
    }

    public async Task<ShelfLocation?> GetByCodeAsync(string code, Guid libraryId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(sl => sl.Code == code && sl.LibraryId == libraryId);
    }

    public async Task<IEnumerable<ShelfLocation>> GetByLibraryAsync(Guid libraryId)
    {
        return await _dbSet
            .Where(sl => sl.LibraryId == libraryId)
            .OrderBy(sl => sl.Code)
            .ToListAsync();
    }
}
