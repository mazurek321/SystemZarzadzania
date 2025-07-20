namespace Core.Models.UserTasks;

public interface IUserTaskRepository
{
    Task CreateAsync(UserTask task);
    Task UpdateAsync(UserTask task);
    Task<UserTask> GetByIdAsync(Guid id);
    Task<List<UserTask>> GetWithComingDeadlinesAsync(DateTime time, Guid userId);
    Task<ICollection<UserTask>> BrowseTasks(int pageNumber, int pageSize, Guid? userId, List<int>? categories);
    Task<ICollection<UserTask>> GetCompletedTasks(DateTime? from, DateTime? to, Guid? userId);
    Task DeleteAsync(UserTask task);
}