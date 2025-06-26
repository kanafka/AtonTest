using Domain.Entytis;

namespace UserManagement.Application.DTOs;

public record CreateUserRequest(
    string Login,
    string Password,
    string Name,
    Gender Gender,
    DateTime? Birthday,
    bool Admin
);

public record UpdatePersonalInfoRequest(
    string Name,
    Gender Gender,
    DateTime? Birthday
);

public record UpdatePasswordRequest(string Password);

public record UpdateLoginRequest(string Login);

public record AuthenticateRequest(string Login, string Password);

public record UserResponse(
    Guid Id,
    string Login,
    string Name,
    Gender Gender,
    DateTime? Birthday,
    bool Admin,
    bool IsActive,
    DateTime CreatedOn,
    string CreatedBy,
    DateTime ModifiedOn,
    string ModifiedBy,
    DateTime? RevokedOn,
    string? RevokedBy
)
{
    public UserResponse(User user) : this(
        user.Id,
        user.Login,
        user.Name,
        user.Gender,
        user.Birthday,
        user.Admin,
        user.IsActive,
        user.CreatedOn,
        user.CreatedBy,
        user.ModifiedOn,
        user.ModifiedBy,
        user.RevokedOn,
        user.RevokedBy
    ) { }
}

public record UserBasicResponse(
    string Name,
    Gender Gender,
    DateTime? Birthday,
    bool IsActive
)
{
    public UserBasicResponse(User user) : this(
        user.Name,
        user.Gender,
        user.Birthday,
        user.IsActive
    ) { }
}

public record AuthenticatedUserResponse(
    Guid Id,
    string Login,
    string Name,
    Gender Gender,
    DateTime? Birthday,
    bool Admin
)
{
    public AuthenticatedUserResponse(User user) : this(
        user.Id,
        user.Login,
        user.Name,
        user.Gender,
        user.Birthday,
        user.Admin
    ) { }
}