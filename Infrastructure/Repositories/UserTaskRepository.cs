using Infrastructure.Database;
using Core.Models.UserTasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal sealed class UserTaskRepository(AppDbContext dbContext) : IUserTaskRepository
{
    public async Task CreateAsync(UserTask task)
    {
        dbContext.Tasks.Add(task);
        await dbContext.SaveChangesAsync();
    }
    public async Task UpdateAsync(UserTask task)
    {
        dbContext.Tasks.Update(task);
        await dbContext.SaveChangesAsync();
    }
    public async Task<UserTask> GetByIdAsync(Guid id)
    {
        return await dbContext.Tasks.FirstOrDefaultAsync(x=>x.Id == id);
    }
    public async Task<ICollection<UserTask>> BrowseTasks(int pageNumber, int pageSize, Guid? userId, List<int>? categories)
    {
        var query = dbContext.Tasks
                        .Include(x => x.Users)
                        .Include(x => x.Categories)
                        .AsQueryable();

        if (userId is not null)
            query = query.Where(x => x.Users.Any(u=>u.Id == userId) || x.CreatedBy == userId);

        if (categories is not null && categories.Any())
            query = query.Where(x => x.Categories.Any(c => categories.Contains(c.Id)));

        var result = await query
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();
        return result;
        
    }
    public async Task DeleteAsync(UserTask task)
    {
        dbContext.Tasks.Remove(task);
        await dbContext.SaveChangesAsync();
    }
}