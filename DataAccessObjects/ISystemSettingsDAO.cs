using BusinessObjects;

namespace DataAccessObjects;

public interface ISystemSettingsDAO : IBaseDAO<SystemSettings>
{
    Task<SystemSettings?> GetByLibraryAsync(Guid libraryId);
}
