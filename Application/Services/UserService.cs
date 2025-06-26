using Domain.Entytis;
using Domain.Interfaces;
using UserManagement.Application.DTOs;

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
        if (user == null || !_hasher.Verify(password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid login or password");

        return user;
    }

    private async Task<User> AuthenticateAdminAsync(string login, string password)
    {
        var user = await AuthenticateAsync(login, password);
        if (!user.IsAdmin)
            throw new UnauthorizedAccessException("Access denied: not an admin");

        return user;
    }

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request, string adminLogin, string adminPassword)
    {
        await AuthenticateAdminAsync(adminLogin, adminPassword);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Login = request.Login,
            PasswordHash = _hasher.Hash(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth,
            IsAdmin = request.IsAdmin,
            IsActive = true
        };

        await _repository.AddAsync(user);
        return new UserResponse(user);
    }

    public async Task<UserResponse> UpdatePersonalInfoAsync(Guid userId, UpdatePersonalInfoRequest request,
        string login, string password)
    {
        var user = await AuthenticateAsync(login, password);
        if (user.Id != userId && !user.IsAdmin)
            throw new UnauthorizedAccessException("You are not allowed to update this user's information");

        var toUpdate = await _repository.GetByIdAsync(userId) ?? throw new Exception("User not found");

        toUpdate.FirstName = request.FirstName;
        toUpdate.LastName = request.LastName;
        toUpdate.DateOfBirth = request.DateOfBirth;

        await _repository.UpdateAsync(toUpdate);
        return new UserResponse(toUpdate);
    }

    public async Task<UserResponse> UpdatePasswordAsync(Guid userId, UpdatePasswordRequest request, string login,
        string password)
    {
        var user = await AuthenticateAsync(login, password);
        if (user.Id != userId && !user.IsAdmin)
            throw new UnauthorizedAccessException("You are not allowed to update this user's password");

        var toUpdate = await _repository.GetByIdAsync(userId) ?? throw new Exception("User not found");

        toUpdate.PasswordHash = _hasher.Hash(request.NewPassword);

        await _repository.UpdateAsync(toUpdate);
        return new UserResponse(toUpdate);
    }

    public async Task<UserResponse> UpdateLoginAsync(Guid userId, UpdateLoginRequest request, string login,
        string password)
    {
        var user = await AuthenticateAsync(login, password);
        if (user.Id != userId && !user.IsAdmin)
            throw new UnauthorizedAccessException("You are not allowed to update this user's login");

        var toUpdate = await _repository.GetByIdAsync(userId) ?? throw new Exception("User not found");

        toUpdate.Login = request.NewLogin;
        await _repository.UpdateAsync(toUpdate);
        return new UserResponse(toUpdate);
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
        var user = await _repository.GetByLoginAsync(login);
        if (user == null || !_hasher.Verify(password, user.PasswordHash) || !user.IsActive)
            return null;

        return new AuthenticatedUserResponse(user.Id, user.Login, user.IsAdmin);
    }

    public async Task<List<UserResponse>> GetUsersByMinAgeAsync(int minAge, string adminLogin, string adminPassword)
    {
        await AuthenticateAdminAsync(adminLogin, adminPassword);
        var users = await _repository.GetUsersByMinAgeAsync(minAge);
        return users.Select(u => new UserResponse(u)).ToList();
    }

    public async Task DeleteUserAsync(string loginToDelete, string adminLogin, string adminPassword, bool softDelete)
    {
        await AuthenticateAdminAsync(adminLogin, adminPassword);
        var user = await _repository.GetByLoginAsync(loginToDelete) ?? throw new Exception("User not found");

        if (softDelete)
        {
            user.IsActive = false;
            await _repository.UpdateAsync(user);
        }
        else
        {
            await _repository.DeleteAsync(user);
        }
    }

    public async Task<UserResponse> RestoreUserAsync(string loginToRestore, string adminLogin, string adminPassword)
    {
        await AuthenticateAdminAsync(adminLogin, adminPassword);
        var user = await _repository.GetByLoginAsync(loginToRestore) ?? throw new Exception("User not found");
        user.IsActive = true;
        await _repository.UpdateAsync(user);
        return new UserResponse(user);
    }
}