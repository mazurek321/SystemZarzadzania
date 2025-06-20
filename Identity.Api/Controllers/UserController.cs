using System.Security.Claims;
using Database.Data;
using Identity.Api.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/auth/user")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    public UserController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetUser()
    {
        var userId = User.FindFirst("userid")?.Value;

        Guid userGuidId;
        bool success = Guid.TryParse(userId, out userGuidId);

        if (success)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst("userrrole")?.Value;

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userGuidId);
            
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

        return BadRequest("Something went wrong.");
    }
}