using BusinessObjects;

namespace Services;

public interface IUserService
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    IQueryable<User> GetAllAsQueryable();
    Task<User> CreateUserAsync(User user, string password);
    Task<User> CreateUserWithRolesAsync(User user, string password, List<string> roleNames);
    Task<User> UpdateUserAsync(User user);
    Task<User> UpdateUserWithRolesAsync(User user, List<string> roleNames);
    Task<bool> DeleteUserAsync(Guid id);
    Task<bool> LockUserAsync(Guid id);
    Task<bool> UnlockUserAsync(Guid id);
    Task<IEnumerable<User>> GetLibrariansByLibraryAsync(Guid libraryId);
    Task<IEnumerable<User>> GetBorrowersByLibraryAsync(Guid libraryId);
    Task<User?> GetWithRolesAsync(Guid id);
}
