using Core.Models.Users;
using Core.Models.Tokens;
using Api.Dto.AuthDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Database;
using Infrastructure.Services;
using Infrastructure.Email;
using Infrastructure.Email.EmailTypes;
using Core.Domain;
using Core.Events;
using Core.Entities;
using Microsoft.Extensions.Logging;
using System.Text.Json;




namespace Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly ITokenService _tokenService;
    private readonly EmailSender _emailSender;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        AppDbContext dbContext,
        ITokenService tokenService,
        EmailSender emailSender,
        IUserRepository userRepository,
        ILogger<AuthController> logger
    )
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
        _emailSender = emailSender;
        _userRepository = userRepository;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var email = dto.Email;

        var user = await _userRepository.FindByEmailAsync(email);

        if (user is null)
            return NotFound("User with this email does not exist.");

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

        await _userRepository.UpdateActivityAsync(user);

         _logger.LogInformation("[Login] User {UserId} logged in.", user.Id);

        return Ok(token);
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var hasher = new PasswordHasher<User>();

        var exists = await _userRepository.FindByEmailAsync(dto.Email);
        if (exists is not null)
            return Conflict("User with this email already exists.");

        var user = Core.Models.Users.User.CreateUser(
                dto.Name,
                dto.Lastname,
                "temp",
                dto.Email,
                dto.Phone
            );
            
        var hashedPassword = hasher.HashPassword(user, dto.Password);
        user.SetPasswordHash(hashedPassword);

        var userEvent = new UserWithEvents(user);
        userEvent.AddDomainEvent(new UserRegisteredEvent(userEvent));

        await _userRepository.AddAsync(userEvent);

        _logger.LogInformation("[Registration] Created user {UserId}.", userEvent.Id);

        return Ok(new { Id = userEvent.Id });

    }

}