using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public class RoleDAO : BaseDAO<Role>, IRoleDAO
{
    private readonly FULibraryDbContext _roleContext;

    public RoleDAO(FULibraryDbContext context) : base(context)
    {
        _roleContext = context;
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _roleContext.Roles
            .FirstOrDefaultAsync(r => r.Name == name);
    }
}
