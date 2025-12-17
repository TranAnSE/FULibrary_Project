using BusinessObjects;
using DataAccessObjects;

namespace Repositories;

public class ShelfLocationRepository : Repository<ShelfLocation>, IShelfLocationRepository
{
    private readonly IShelfLocationDAO _shelfLocationDAO;

    public ShelfLocationRepository(IShelfLocationDAO shelfLocationDAO) : base(shelfLocationDAO)
    {
        _shelfLocationDAO = shelfLocationDAO;
    }

    public async Task<ShelfLocation?> GetByCodeAsync(string code, Guid libraryId)
    {
        return await _shelfLocationDAO.GetByCodeAsync(code, libraryId);
    }

    public async Task<IEnumerable<ShelfLocation>> GetByLibraryAsync(Guid libraryId)
    {
        return await _shelfLocationDAO.GetByLibraryAsync(libraryId);
    }
}
