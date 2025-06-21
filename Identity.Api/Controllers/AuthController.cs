using Database.Data;
using Database.Data.Models;
using Identity.Api.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Email;
using Infrastructure.Email.EmailTypes;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class LoginController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly ITokenService _tokenService;
    private readonly EmailSender _emailSender;


    public LoginController(
        AppDbContext dbContext,
        ITokenService tokenService,
        EmailSender emailSender
    )
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
        _emailSender = emailSender;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var email = dto.Email;

        var user = await _dbContext.Users
                        .FirstOrDefaultAsync(x => x.Email == email);

        if (user is null)
            return Unauthorized("User with this email does not exist.");

        var inputpassword = dto.Password;

        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, inputpassword);

        if (result != PasswordVerificationResult.Success)
            return Unauthorized("Password incorrect.");

        var request = new TokenGenerationRequest
        {
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role
        };

        var token = _tokenService.GenerateToken(request);

        return Ok(token);
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var hasher = new PasswordHasher<User>();

        var exists = await _dbContext.Users.AnyAsync(x => x.Email == dto.Email);
        if (exists)
            return Conflict("User with this email already exists.");

        var user = Database.Data.Models.User.CreateUser(
                dto.Name, dto.Lastname, "temp", dto.Email
            );
            
        var hashedPassword = hasher.HashPassword(user, dto.Password);
        user.SetPasswordHash(hashedPassword);

        var message = "Thanks for creating account!";

        await _emailSender.SendEmailAsync(user.Email, user.Email, EmailType.Registration);

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return Ok("User registered successfully.");
    }

}