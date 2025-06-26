using Infrastructure.Database;
using Core.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal sealed class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task AddAsync(User user)
    {
        dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
    }

    public async Task<User> FindByEmailAsync(string email)
    {
        return await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
    }
}