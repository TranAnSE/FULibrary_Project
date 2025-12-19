using BusinessObjects;
using DataAccessObjects;
using Repositories;

namespace Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleDAO _roleDAO;
    private readonly IUserRoleDAO _userRoleDAO;
    private readonly IAuthService _authService;

    public UserService(IUserRepository userRepository, IRoleDAO roleDAO, IUserRoleDAO userRoleDAO, IAuthService authService)
    {
        _userRepository = userRepository;
        _roleDAO = roleDAO;
        _userRoleDAO = userRoleDAO;
        _authService = authService;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public IQueryable<User> GetAllAsQueryable()
    {
        return _userRepository.GetAllAsQueryable();
    }

    public async Task<User> CreateUserAsync(User user, string password)
    {
        user.PasswordHash = _authService.HashPassword(password);
        user.MustChangePassword = true;
        return await _userRepository.CreateAsync(user);
    }

    public async Task<User> CreateUserWithRolesAsync(User user, string password, List<string> roleNames)
    {
        user.PasswordHash = _authService.HashPassword(password);
        user.MustChangePassword = true;
        
        var createdUser = await _userRepository.CreateAsync(user);
        
        // Assign roles
        if (roleNames != null && roleNames.Any())
        {
            foreach (var roleName in roleNames)
            {
                var role = await _roleDAO.GetByNameAsync(roleName);
                if (role != null)
                {
                    var userRole = new UserRole
                    {
                        UserId = createdUser.Id,
                        RoleId = role.Id
                    };
                    await _userRoleDAO.AddAsync(userRole);
                }
            }
        }
        
        return createdUser;
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        return await _userRepository.UpdateAsync(user);
    }

    public async Task<User> UpdateUserWithRolesAsync(User user, List<string> roleNames)
    {
        // Update user basic info
        var updatedUser = await _userRepository.UpdateAsync(user);
        
        // Update roles
        // Remove existing roles
        var existingUserRoles = await _userRoleDAO.GetByUserIdAsync(user.Id);
        foreach (var userRole in existingUserRoles)
        {
            await _userRoleDAO.DeleteAsync(userRole.Id);
        }
        
        // Add new roles
        if (roleNames != null && roleNames.Any())
        {
            foreach (var roleName in roleNames)
            {
                var role = await _roleDAO.GetByNameAsync(roleName);
                if (role != null)
                {
                    var userRole = new UserRole
                    {
                        UserId = user.Id,
                        RoleId = role.Id
                    };
                    await _userRoleDAO.AddAsync(userRole);
                }
            }
        }
        
        return updatedUser;
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        return await _userRepository.SoftDeleteAsync(id);
    }

    public async Task<bool> LockUserAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return false;

        user.IsLocked = true;
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<bool> UnlockUserAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return false;

        user.IsLocked = false;
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<IEnumerable<User>> GetLibrariansByLibraryAsync(Guid libraryId)
    {
        return await _userRepository.GetLibrariansByLibraryAsync(libraryId);
    }

    public async Task<IEnumerable<User>> GetBorrowersByLibraryAsync(Guid libraryId)
    {
        return await _userRepository.GetBorrowersByLibraryAsync(libraryId);
    }

    public async Task<User?> GetWithRolesAsync(Guid id)
    {
        return await _userRepository.GetWithRolesAsync(id);
    }
}
