using System.Security.Claims;
using Infrastructure.Database;
using Infrastructure.Context;
using Api.Dto.UserDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Core.Models.Users;

namespace Api.Controllers;

[ApiController]
[Route("api/auth/user")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly ICurrentUserService _user;
    public UserController(AppDbContext dbContext, ICurrentUserService user)
    {
        _dbContext = dbContext;
        _user = user;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetMe()
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == _user.Id);

        if (user is null)
            return NotFound("User not found.");

        return Ok(new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Lastname = user.Lastname,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        });
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto dto)
    {
        var updatingUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == _user.Id);

        if (updatingUser is null)
            return NotFound("User not found.");

        updatingUser.UpdateUser(
            dto.Name,
            dto.Lastname,
            dto.Email
        );

        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpPut("password")]
    [Authorize]
    public async Task<IActionResult> UpdateUserPassword([FromBody] UpdateUserPasswordDto dto)
    {
        var updatingUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == _user.Id);

        if (updatingUser is null)
            return NotFound("User not found.");
            
        var hasher = new PasswordHasher<User>();

        var result = hasher.VerifyHashedPassword(updatingUser, updatingUser.PasswordHash, dto.OldPassword);

        if (result != PasswordVerificationResult.Success)
            return Unauthorized("Old password incorrect.");

        var hashedPassword = hasher.HashPassword(updatingUser, dto.NewPassword);
        updatingUser.SetPasswordHash(hashedPassword);

        await _dbContext.SaveChangesAsync();
        return Ok();
    }
}