using Domain.Entytis;
using Domain.Interfaces;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;

namespace UserManagement.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IPasswordHasher _hasher;
    
    public UserService(IUserRepository repository, IPasswordHasher hasher)
    {
        _repository = repository;
        _hasher = hasher;
    }

    private async Task<User> AuthenticateAsync(string login, string password)
    {
        var user = await _repository.GetByLoginAsync(login);
        if (user == null || !_hasher.Verify(password, user.Password))
            throw new UnauthorizedAccessException("Invalid login or password");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("User account is inactive");

        return user;
    }

    private async Task<User> AuthenticateAdminAsync(string login, string password)
    {
        var user = await AuthenticateAsync(login, password);
        if (!user.Admin)
            throw new UnauthorizedAccessException("Access denied: not an admin");

        return user;
    }

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request, string adminLogin, string adminPassword)
    {
        var admin = await AuthenticateAdminAsync(adminLogin, adminPassword);

        // Check if login already exists
        if (await _repository.LoginExistsAsync(request.Login))
            throw new ArgumentException("Login already exists");

        var user = new User(
            request.Login,
            _hasher.Hash(request.Password),
            request.Name,
            request.Gender,
            request.Birthday,
            request.Admin,
            admin.Login
        );

        await _repository.AddAsync(user);
        await _repository.SaveChangesAsync();
        return new UserResponse(user);
    }

    public async Task<UserResponse> UpdatePersonalInfoAsync(Guid userId, UpdatePersonalInfoRequest request,
        string login, string password)
    {
        var authenticatedUser = await AuthenticateAsync(login, password);
        var userToUpdate = await _repository.GetByIdAsync(userId) ?? throw new ArgumentException("User not found");

        // Check permissions: admin or the user themselves
        if (authenticatedUser.Id != userId && !authenticatedUser.Admin)
            throw new UnauthorizedAccessException("You are not allowed to update this user's information");

        // Check if user to update is active (unless admin is updating)
        if (!authenticatedUser.Admin && !userToUpdate.IsActive)
            throw new UnauthorizedAccessException("Cannot update inactive user");

        userToUpdate.UpdatePersonalInfo(request.Name, request.Gender, request.Birthday, authenticatedUser.Login);
        await _repository.UpdateAsync(userToUpdate);
        await _repository.SaveChangesAsync();
        return new UserResponse(userToUpdate);
    }

    public async Task<UserResponse> UpdatePasswordAsync(Guid userId, UpdatePasswordRequest request, string login,
        string password)
    {
        var authenticatedUser = await AuthenticateAsync(login, password);
        var userToUpdate = await _repository.GetByIdAsync(userId) ?? throw new ArgumentException("User not found");

        // Check permissions: admin or the user themselves
        if (authenticatedUser.Id != userId && !authenticatedUser.Admin)
            throw new UnauthorizedAccessException("You are not allowed to update this user's password");

        // Check if user to update is active (unless admin is updating)
        if (!authenticatedUser.Admin && !userToUpdate.IsActive)
            throw new UnauthorizedAccessException("Cannot update inactive user");

        userToUpdate.UpdatePassword(_hasher.Hash(request.Password), authenticatedUser.Login);
        await _repository.UpdateAsync(userToUpdate);
        await _repository.SaveChangesAsync();
        return new UserResponse(userToUpdate);
    }

    public async Task<UserResponse> UpdateLoginAsync(Guid userId, UpdateLoginRequest request, string login,
        string password)
    {
        var authenticatedUser = await AuthenticateAsync(login, password);
        var userToUpdate = await _repository.GetByIdAsync(userId) ?? throw new ArgumentException("User not found");

        // Check permissions: admin or the user themselves
        if (authenticatedUser.Id != userId && !authenticatedUser.Admin)
            throw new UnauthorizedAccessException("You are not allowed to update this user's login");

        // Check if user to update is active (unless admin is updating)
        if (!authenticatedUser.Admin && !userToUpdate.IsActive)
            throw new UnauthorizedAccessException("Cannot update inactive user");

        // Check if new login already exists
        if (await _repository.LoginExistsAsync(request.Login, userId))
            throw new ArgumentException("Login already exists");

        userToUpdate.UpdateLogin(request.Login, authenticatedUser.Login);
        await _repository.UpdateAsync(userToUpdate);
        await _repository.SaveChangesAsync();
        return new UserResponse(userToUpdate);
    }

    public async Task<List<UserResponse>> GetAllActiveUsersAsync(string adminLogin, string adminPassword)
    {
        await AuthenticateAdminAsync(adminLogin, adminPassword);
        var users = await _repository.GetAllActiveAsync();
        return users.Select(u => new UserResponse(u)).ToList();
    }

    public async Task<UserBasicResponse?> GetUserByLoginAsync(string loginToFind, string adminLogin,
        string adminPassword)
    {
        await AuthenticateAdminAsync(adminLogin, adminPassword);
        var user = await _repository.GetByLoginAsync(loginToFind);
        return user == null ? null : new UserBasicResponse(user);
    }

    public async Task<AuthenticatedUserResponse?> AuthenticateUserAsync(string login, string password)
    {
        try
        {
            var user = await AuthenticateAsync(login, password);
            return new AuthenticatedUserResponse(user);
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
    }

    public async Task<List<UserResponse>> GetUsersByMinAgeAsync(int minAge, string adminLogin, string adminPassword)
    {
        await AuthenticateAdminAsync(adminLogin, adminPassword);
        var users = await _repository.GetUsersByMinAgeAsync(minAge);
        return users.Select(u => new UserResponse(u)).ToList();
    }

    public async Task DeleteUserAsync(string loginToDelete, string adminLogin, string adminPassword, bool softDelete)
    {
        var admin = await AuthenticateAdminAsync(adminLogin, adminPassword);
        var user = await _repository.GetByLoginAsync(loginToDelete) ?? throw new ArgumentException("User not found");

        if (softDelete)
        {
            user.SoftDelete(admin.Login);
            await _repository.UpdateAsync(user);
        }
        else
        {
            await _repository.DeleteAsync(user);
        }
        await _repository.SaveChangesAsync();
    }

    public async Task<UserResponse> RestoreUserAsync(string loginToRestore, string adminLogin, string adminPassword)
    {
        var admin = await AuthenticateAdminAsync(adminLogin, adminPassword);
        var user = await _repository.GetByLoginAsync(loginToRestore) ?? throw new ArgumentException("User not found");
        
        user.Restore(admin.Login);
        await _repository.UpdateAsync(user);
        await _repository.SaveChangesAsync();
        return new UserResponse(user);
    }
}