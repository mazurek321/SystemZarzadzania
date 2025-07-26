using Infrastructure.Database;
using Core.Models.Users;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace Infrastructure.Context;

public class AdminSeeder
{
    private readonly AppDbContext _dbContext;
    private readonly IUserRepository _userRepository;

    public AdminSeeder(AppDbContext dbContext, IUserRepository userRepository)
    {
        _dbContext = dbContext;
        _userRepository = userRepository;
    }

    public async Task SeedAdminAsync()
    {
        if (!await _dbContext.Users.AnyAsync(u => u.Email == "admin@admin.com"))
        {
            var hasher = new PasswordHasher<User>();

            var admin = User.CreateAdmin(
                "Admin",
                "Admin",
                "admin",
                "admin@admin.com",
                "123456789"
            );

            var password = "admin";
            var hashedPassword = hasher.HashPassword(admin, password);
            admin.SetPasswordHash(hashedPassword);

            var adminEvents = new UserWithEvents(admin);

            await _userRepository.AddAsync(adminEvents);
        }
    }
}