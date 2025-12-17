using BusinessObjects;

namespace Services;

public interface IUserService
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> CreateUserAsync(User user, string password);
    Task<User> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(Guid id);
    Task<bool> LockUserAsync(Guid id);
    Task<bool> UnlockUserAsync(Guid id);
    Task<IEnumerable<User>> GetLibrariansByLibraryAsync(Guid libraryId);
    Task<IEnumerable<User>> GetBorrowersByLibraryAsync(Guid libraryId);
    Task<User?> GetWithRolesAsync(Guid id);
}
