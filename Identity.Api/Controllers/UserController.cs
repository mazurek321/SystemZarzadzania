using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/auth/user")]
public class UserController : ControllerBase
{

    [Authorize]
    [HttpGet]
    public IActionResult GetUserInfo()
    {
        var userId = User.FindFirst("userid")?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        return Ok(new { userId, email, role });
    }
}