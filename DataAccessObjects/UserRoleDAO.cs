using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public class UserRoleDAO : BaseDAO<UserRole>, IUserRoleDAO
{
    private readonly FULibraryDbContext _userRoleContext;

    public UserRoleDAO(FULibraryDbContext context) : base(context)
    {
        _userRoleContext = context;
    }

    public async Task<IEnumerable<UserRole>> GetByUserIdAsync(Guid userId)
    {
        return await _userRoleContext.UserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserRole>> GetByRoleIdAsync(Guid roleId)
    {
        return await _userRoleContext.UserRoles
            .Include(ur => ur.User)
            .Where(ur => ur.RoleId == roleId)
            .ToListAsync();
    }
}
