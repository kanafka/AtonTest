using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> CreateUser(
        [FromBody] CreateUserRequest request,
        [FromHeader(Name = "AdminLogin")] string adminLogin,
        [FromHeader(Name = "AdminPassword")] string adminPassword)
    {
        try
        {
            var result = await _userService.CreateUserAsync(request, adminLogin, adminPassword);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPut("{userId}/personal-info")]
    public async Task<ActionResult<UserResponse>> UpdatePersonalInfo(
        Guid userId,
        [FromBody] UpdatePersonalInfoRequest request,
        [FromHeader(Name = "Login")] string login,
        [FromHeader(Name = "Password")] string password)
    {
        try
        {
            var result = await _userService.UpdatePersonalInfoAsync(userId, request, login, password);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPut("{userId}/password")]
    public async Task<ActionResult<UserResponse>> UpdatePassword(
        Guid userId,
        [FromBody] UpdatePasswordRequest request,
        [FromHeader(Name = "Login")] string login,
        [FromHeader(Name = "Password")] string password)
    {
        try
        {
            var result = await _userService.UpdatePasswordAsync(userId, request, login, password);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPut("{userId}/login")]
    public async Task<ActionResult<UserResponse>> UpdateLogin(
        Guid userId,
        [FromBody] UpdateLoginRequest request,
        [FromHeader(Name = "Login")] string login,
        [FromHeader(Name = "Password")] string password)
    {
        try
        {
            var result = await _userService.UpdateLoginAsync(userId, request, login, password);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpGet("active")]
    public async Task<ActionResult<List<UserResponse>>> GetAllActiveUsers(
        [FromHeader(Name = "AdminLogin")] string adminLogin,
        [FromHeader(Name = "AdminPassword")] string adminPassword)
    {
        try
        {
            var result = await _userService.GetAllActiveUsersAsync(adminLogin, adminPassword);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpGet("by-login/{login}")]
    public async Task<ActionResult<UserBasicResponse>> GetUserByLogin(
        string login,
        [FromHeader(Name = "AdminLogin")] string adminLogin,
        [FromHeader(Name = "AdminPassword")] string adminPassword)
    {
        try
        {
            var result = await _userService.GetUserByLoginAsync(login, adminLogin, adminPassword);
            return result == null ? NotFound(new { Error = "User not found" }) : Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("authenticate")]
    public async Task<ActionResult<AuthenticatedUserResponse>> Authenticate([FromBody] AuthenticateRequest request)
    {
        try
        {
            var result = await _userService.AuthenticateUserAsync(request.Login, request.Password);
            return result == null ? Unauthorized(new { Error = "Invalid credentials or inactive user" }) : Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpGet("by-age/{minAge}")]
    public async Task<ActionResult<List<UserResponse>>> GetUsersByMinAge(
        int minAge,
        [FromHeader(Name = "AdminLogin")] string adminLogin,
        [FromHeader(Name = "AdminPassword")] string adminPassword)
    {
        try
        {
            var result = await _userService.GetUsersByMinAgeAsync(minAge, adminLogin, adminPassword);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpDelete("{login}")]
    public async Task<ActionResult> DeleteUser(
        string login,
        [FromQuery] bool softDelete,
        [FromHeader(Name = "AdminLogin")] string adminLogin,
        [FromHeader(Name = "AdminPassword")] string adminPassword)
    {
        try
        {
            await _userService.DeleteUserAsync(login, adminLogin, adminPassword, softDelete);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("{login}/restore")]
    public async Task<ActionResult<UserResponse>> RestoreUser(
        string login,
        [FromHeader(Name = "AdminLogin")] string adminLogin,
        [FromHeader(Name = "AdminPassword")] string adminPassword)
    {
        try
        {
            var result = await _userService.RestoreUserAsync(login, adminLogin, adminPassword);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}