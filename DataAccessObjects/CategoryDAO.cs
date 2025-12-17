using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public class CategoryDAO : BaseDAO<Category>, ICategoryDAO
{
    public CategoryDAO(FULibraryDbContext context) : base(context)
    {
    }

    public async Task<Category?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Name == name);
    }
}
