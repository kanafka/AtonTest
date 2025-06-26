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
);

public record UserBasicResponse(
    string Name,
    Gender Gender,
    DateTime? Birthday,
    bool IsActive
);

public record AuthenticatedUserResponse(
    Guid Id,
    string Login,
    string Name,
    Gender Gender,
    DateTime? Birthday,
    bool Admin
);