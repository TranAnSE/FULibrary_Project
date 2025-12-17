using BusinessObjects;

namespace DataAccessObjects;

public interface IUserDAO : IBaseDAO<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByCardNumberAsync(string cardNumber);
    Task<IEnumerable<User>> GetByRoleAsync(Guid roleId);
    Task<IEnumerable<User>> GetLibrariansByLibraryAsync(Guid libraryId);
    Task<IEnumerable<User>> GetBorrowersByLibraryAsync(Guid libraryId);
    Task<User?> GetWithRolesAsync(Guid id);
}
