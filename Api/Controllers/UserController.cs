using System.Security.Claims;
using Infrastructure.Database;
using Infrastructure.Context;
using Api.Dto.UserDtos;
using Api.Dto.TaskDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Core.Models.Users;
using Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using System.Text.Json;



namespace Api.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _user;
    private readonly ILogger<UserController> _logger;
    public UserController(AppDbContext dbContext, ICurrentUserService user, IUserRepository userRepository, ILogger<UserController> logger)
    {
        _dbContext = dbContext;
        _user = user;
        _userRepository = userRepository;
        _logger = logger;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetUser([FromQuery] Guid? userId)
    {
        var query = userId is null ? _user.Id.Value : userId.Value;
        var user = await _userRepository.FindByIdAsync(query);

        if (user is null)
            return NotFound("User not found.");

        return Ok(new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Lastname = user.Lastname,
            Email = user.Email,
            Phone = user.Phone,
            IsActive = user.IsActive,
            LastActive = user.LastActive,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
        });
    }

    [HttpGet("browse")]
    [Authorize]
    public async Task<IActionResult> BrowseUsers([FromQuery] int pageNumber = 1, int pageSize = 25)
    {
        var users = await _userRepository.BrowseUsers(pageNumber, pageSize);
        var userDtos = users.Select(user => new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Lastname = user.Lastname,
            Email = user.Email,
            Phone = user.Phone,
            IsActive = user.IsActive,
            LastActive = user.LastActive,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
        }).ToList();

        return Ok(userDtos);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto dto)
    {
        var updatingUser = await _userRepository.FindByEmailAsync(dto.Email);

        if (updatingUser is null)
            return NotFound("User not found.");

        if (updatingUser.Id != _user.Id)
            return BadRequest("You cannot update this user.");

        _logger.LogInformation("[Update] User {UserId} updated", updatingUser.Id, DateTime.Now);
        _logger.LogInformation("[UpdateData] User {UserId} updated from {userData}.", updatingUser.Id, JsonSerializer.Serialize(updatingUser));

        updatingUser.UpdateUser(
            dto.Name,
            dto.Lastname,
            dto.Email,
            dto.Phone
        );

        await _userRepository.UpdateInformationAsync(updatingUser);

        _logger.LogInformation("[UpdateData] User {UserId} updated to {userData}.", updatingUser.Id, JsonSerializer.Serialize(updatingUser));

        return Ok(updatingUser);
    }

    [HttpPut("password")]
    [Authorize]
    public async Task<IActionResult> UpdateUserPassword([FromBody] UpdateUserPasswordDto dto)
    {
        var updatingUser = await _userRepository.FindByIdAsync(_user.Id.Value);

        if (updatingUser is null)
            return NotFound("User not found.");

        if (updatingUser.Id != _user.Id)
            return BadRequest("You cannot change password of this user.");

        var hasher = new PasswordHasher<User>();

        var result = hasher.VerifyHashedPassword(updatingUser, updatingUser.PasswordHash, dto.OldPassword);

        if (result != PasswordVerificationResult.Success)
            return Unauthorized("Old password incorrect.");

        var hashedPassword = hasher.HashPassword(updatingUser, dto.NewPassword);

        updatingUser.SetPasswordHash(hashedPassword);
        await _userRepository.UpdateInformationAsync(updatingUser);

         _logger.LogInformation("[UpdatePassword] User {UserId} updated password.", updatingUser.Id);

        return Ok();
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteUser()
    {
        var user = await _userRepository.FindByIdAsync(_user.Id.Value);

        if (user is null)
            return NotFound("User not found.");

        await _userRepository.DeleteUserAsync(user);

         _logger.LogInformation("[Delete] User deleted account: {user}.", JsonSerializer.Serialize(user));

        return NoContent();
    }
}