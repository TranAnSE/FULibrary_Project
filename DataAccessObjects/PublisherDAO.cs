using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public class PublisherDAO : BaseDAO<Publisher>, IPublisherDAO
{
    public PublisherDAO(FULibraryDbContext context) : base(context)
    {
    }

    public async Task<Publisher?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Name == name);
    }
}
