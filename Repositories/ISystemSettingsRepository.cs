using BusinessObjects;

namespace Repositories;

public interface ISystemSettingsRepository : IRepository<SystemSettings>
{
    Task<SystemSettings?> GetByLibraryAsync(Guid libraryId);
}
