using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public class LibraryDAO : BaseDAO<Library>, ILibraryDAO
{
    public LibraryDAO(FULibraryDbContext context) : base(context)
    {
    }

    public new IQueryable<Library> GetAllAsQueryable()
    {
        return _dbSet;
    }

    public async Task<Library?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(l => l.Name == name);
    }

    public async Task<Library?> GetWithBooksAsync(Guid id)
    {
        return await _dbSet
            .Include(l => l.Books)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<Library?> GetWithSettingsAsync(Guid id)
    {
        return await _dbSet
            .Include(l => l.Settings)
            .FirstOrDefaultAsync(l => l.Id == id);
    }
}
