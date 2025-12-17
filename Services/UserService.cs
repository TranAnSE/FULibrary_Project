using BusinessObjects;
using Repositories;

namespace Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;

    public UserService(IUserRepository userRepository, IAuthService authService)
    {
        _userRepository = userRepository;
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

    public async Task<User> CreateUserAsync(User user, string password)
    {
        user.PasswordHash = _authService.HashPassword(password);
        user.MustChangePassword = true;
        return await _userRepository.CreateAsync(user);
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        return await _userRepository.UpdateAsync(user);
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
