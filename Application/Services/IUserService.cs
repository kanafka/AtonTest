using UserManagement.Application.DTOs;

public interface IUserService
{
    Task<UserResponse> CreateUserAsync(CreateUserRequest request, string adminLogin, string adminPassword);
    Task<UserResponse> UpdatePersonalInfoAsync(Guid userId, UpdatePersonalInfoRequest request, string login, string password);
    Task<UserResponse> UpdatePasswordAsync(Guid userId, UpdatePasswordRequest request, string login, string password);
    Task<UserResponse> UpdateLoginAsync(Guid userId, UpdateLoginRequest request, string login, string password);
    Task<List<UserResponse>> GetAllActiveUsersAsync(string adminLogin, string adminPassword);
    Task<UserBasicResponse?> GetUserByLoginAsync(string loginToFind, string adminLogin, string adminPassword);
    Task<AuthenticatedUserResponse?> AuthenticateUserAsync(string login, string password);
    Task<List<UserResponse>> GetUsersByMinAgeAsync(int minAge, string adminLogin, string adminPassword);
    Task DeleteUserAsync(string loginToDelete, string adminLogin, string adminPassword, bool softDelete);
    Task<UserResponse> RestoreUserAsync(string loginToRestore, string adminLogin, string adminPassword);
}