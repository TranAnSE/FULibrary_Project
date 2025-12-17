using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public class SystemSettingsDAO : BaseDAO<SystemSettings>, ISystemSettingsDAO
{
    public SystemSettingsDAO(FULibraryDbContext context) : base(context)
    {
    }

    public async Task<SystemSettings?> GetByLibraryAsync(Guid libraryId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(ss => ss.LibraryId == libraryId);
    }
}
