using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public class UserDAO : BaseDAO<User>, IUserDAO
{
    public UserDAO(FULibraryDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByCardNumberAsync(string cardNumber)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.CardNumber == cardNumber);
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(Guid roleId)
    {
        return await _dbSet
            .Include(u => u.UserRoles)
            .Where(u => u.UserRoles.Any(ur => ur.RoleId == roleId))
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetLibrariansByLibraryAsync(Guid libraryId)
    {
        return await _dbSet
            .Where(u => u.AssignedLibraryId == libraryId)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetBorrowersByLibraryAsync(Guid libraryId)
    {
        return await _dbSet
            .Where(u => u.HomeLibraryId == libraryId)
            .ToListAsync();
    }

    public async Task<User?> GetWithRolesAsync(Guid id)
    {
        return await _dbSet
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
}
