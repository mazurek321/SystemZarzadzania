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
        return await dbContext.Tasks
                        .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<UserTask>> GetWithComingDeadlinesAsync(DateTime time, Guid userId)
    {
        return await dbContext.Tasks
                                .Where(x=>x.Users.Any(u => u.Id == userId) && x.Deadline <= time)
                                .ToListAsync();                     
    }

    public async Task<ICollection<UserTask>> BrowseTasks(int pageNumber, int pageSize, Guid? userId, List<int>? categories)
    {
        var query = dbContext.Tasks.AsQueryable();

        if (categories is not null && categories.Any())
            query = query.Where(x => x.Categories.Any(c => categories.Contains(c.Id)));

        var tasks = await query.ToListAsync();

        if (userId.HasValue)
            tasks = tasks
                .Where(x => x.Users.Any(u => u.Id == userId.Value) || x.CreatedBy == userId.Value)
                .ToList();

        return tasks
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public async Task<ICollection<UserTask>> GetCompletedTasks(DateTime? from, DateTime? to, Guid? userId)
    {
        var query = dbContext.Tasks
                                .AsNoTracking()
                                .Include(x=>x.Users)
                                .Include(x=>x.Categories)
                                .AsQueryable();

        query = query.Where(x => x.EndDate.HasValue);

        if (userId is not null)
            query = query.Where(
                x =>
                x.CreatedBy == userId.Value
                ||
                x.Users.Any(u => u.Id == userId.Value)
        );

        if (from.HasValue)
            query = query.Where(x => x.EndDate.Value >= from.Value.Date);
        if (to.HasValue)
            query = query.Where(x => x.EndDate.Value < to.Value.Date.AddDays(1));
        
        return await query.ToListAsync();
    }


    public async Task DeleteAsync(UserTask task)
    {
        dbContext.Tasks.Remove(task);
        await dbContext.SaveChangesAsync();
    }
}