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

    public async Task<User> FindByIdAsync(Guid id)
    {
        return await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<User>> FindByIdsAsync(List<Guid> list)
    {
        return await dbContext.Users.Where(x => list.Contains(x.Id)).ToListAsync();
    }

    public async Task<User> FindByEmailAsync(string email)
    {
        return await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<List<User>> BrowseUsers(int pageNumber, int pageSize, bool? isActiveFilter, User.UserRole? roleFilter, string? sortBy)
    {
        var query = dbContext.Users.AsQueryable();

        if(isActiveFilter.HasValue)
            query = query.Where(x => x.IsActive == isActiveFilter.Value);
        
        if(roleFilter.HasValue)
            query = query.Where(x=>x.Role == roleFilter.Value);

        if(sortBy is not null)
                query = sortBy?.ToLower()
                switch
                {
                    "name" => query.OrderBy(x => x.Name),
                    "email" => query.OrderBy(x => x.Email),
                    "createdat" => query.OrderBy(x => x.CreatedAt),
                    "lastname" => query.OrderBy(x => x.Lastname),
                    _ => query.OrderBy(x => x.CreatedAt) 
                };

        var users = await query
                            .AsNoTracking()
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();

        return users;
    }

    public async Task UpdateActivityAsync(User user)
    {
        user.UpdateActivity();
        dbContext.Update(user);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateInformationAsync(User user)
    {
        dbContext.Update(user);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(User user)
    {
        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync();
    }


}