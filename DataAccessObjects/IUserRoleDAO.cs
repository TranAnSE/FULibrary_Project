using BusinessObjects;

namespace DataAccessObjects;

public interface IUserRoleDAO : IBaseDAO<UserRole>
{
    Task<IEnumerable<UserRole>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<UserRole>> GetByRoleIdAsync(Guid roleId);
}
