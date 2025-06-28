namespace Core.Models.UserTasks;

public interface IUserTaskRepository
{
    Task CreateAsync(UserTask task);
    Task UpdateAsync(UserTask task);
    Task<UserTask> GetByIdAsync(Guid id);
    Task<ICollection<UserTask>> BrowseTasks(int pageNumber, int pageSize, Guid? userId, List<int>? categories);
    Task DeleteAsync(UserTask task);
}