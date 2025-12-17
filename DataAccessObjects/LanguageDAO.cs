using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public class LanguageDAO : BaseDAO<Language>, ILanguageDAO
{
    public LanguageDAO(FULibraryDbContext context) : base(context)
    {
    }

    public async Task<Language?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(l => l.Name == name);
    }

    public async Task<Language?> GetByCodeAsync(string code)
    {
        return await _dbSet.FirstOrDefaultAsync(l => l.Code == code);
    }
}
