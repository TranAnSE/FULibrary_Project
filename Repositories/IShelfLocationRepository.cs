using BusinessObjects;

namespace Repositories;

public interface IShelfLocationRepository : IRepository<ShelfLocation>
{
    Task<ShelfLocation?> GetByCodeAsync(string code, Guid libraryId);
    Task<IEnumerable<ShelfLocation>> GetByLibraryAsync(Guid libraryId);
}
