using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public class OtpTokenDAO : BaseDAO<OtpToken>, IOtpTokenDAO
{
    public OtpTokenDAO(FULibraryDbContext context) : base(context)
    {
    }

    public async Task<OtpToken?> GetByEmailAndCodeAsync(string email, string code)
    {
        return await _dbSet
            .FirstOrDefaultAsync(ot => ot.Email == email && ot.Code == code);
    }

    public async Task<IEnumerable<OtpToken>> GetByEmailAsync(string email)
    {
        return await _dbSet
            .Where(ot => ot.Email == email)
            .OrderByDescending(ot => ot.CreatedAt)
            .ToListAsync();
    }
}
