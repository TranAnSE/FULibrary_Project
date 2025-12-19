using BusinessObjects;
using DataAccessObjects;

namespace Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    private readonly IUserDAO _userDAO;

    public UserRepository(IUserDAO userDAO) : base(userDAO)
    {
        _userDAO = userDAO;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _userDAO.GetByEmailAsync(email);
    }

    public async Task<User?> GetByCardNumberAsync(string cardNumber)
    {
        return await _userDAO.GetByCardNumberAsync(cardNumber);
    }

    public new IQueryable<User> GetAllAsQueryable()
    {
        return _userDAO.GetAllAsQueryable();
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(Guid roleId)
    {
        return await _userDAO.GetByRoleAsync(roleId);
    }

    public async Task<IEnumerable<User>> GetLibrariansByLibraryAsync(Guid libraryId)
    {
        return await _userDAO.GetLibrariansByLibraryAsync(libraryId);
    }

    public async Task<IEnumerable<User>> GetBorrowersByLibraryAsync(Guid libraryId)
    {
        return await _userDAO.GetBorrowersByLibraryAsync(libraryId);
    }

    public async Task<User?> GetWithRolesAsync(Guid id)
    {
        return await _userDAO.GetWithRolesAsync(id);
    }
}
