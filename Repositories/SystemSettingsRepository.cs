using BusinessObjects;
using DataAccessObjects;

namespace Repositories;

public class SystemSettingsRepository : Repository<SystemSettings>, ISystemSettingsRepository
{
    private readonly ISystemSettingsDAO _systemSettingsDAO;

    public SystemSettingsRepository(ISystemSettingsDAO systemSettingsDAO) : base(systemSettingsDAO)
    {
        _systemSettingsDAO = systemSettingsDAO;
    }

    public async Task<SystemSettings?> GetByLibraryAsync(Guid libraryId)
    {
        return await _systemSettingsDAO.GetByLibraryAsync(libraryId);
    }
}
