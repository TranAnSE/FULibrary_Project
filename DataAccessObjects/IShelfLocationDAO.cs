using BusinessObjects;

namespace DataAccessObjects;

public interface IShelfLocationDAO : IBaseDAO<ShelfLocation>
{
    Task<ShelfLocation?> GetByCodeAsync(string code, Guid libraryId);
    Task<IEnumerable<ShelfLocation>> GetByLibraryAsync(Guid libraryId);
}
