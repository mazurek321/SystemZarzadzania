using Core.Models.Categories;
using Core.Models.Users;
using System.Text.Json.Serialization;

namespace Core.Models.UserTasks;

public class UserTask
{
    public UserTask() { }
    public UserTask(Guid id, string title, string? description, DateTime deadline,
                    DateTime? startDate, DateTime? endDate, int priority,
                    DateTime createdAt, Guid createdBy,
                    DateTime lastUpdate,Guid updatedBy,
                    List<User>? users, List<Category>? categories)
    {
        Id = id;
        Title = title;
        Description = description;
        Deadline = deadline;
        StartDate = startDate;
        EndDate = endDate;
        Priority = priority;
        CreatedAt = createdAt;
        CreatedBy = createdBy;
        LastUpdate = lastUpdate;
        UpdatedBy = updatedBy;
        Users = users;
        Categories = categories;
    }
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public DateTime Deadline { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public int Priority { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime LastUpdate { get; private set; }
    public Guid UpdatedBy { get; private set; }
    [JsonIgnore]
    public ICollection<User> Users { get; private set; } = new List<User>();
    public List<Category> Categories { get; private set; } = new List<Category>();

    public static UserTask NewTask(
        string title, string? description, DateTime deadline,
        DateTime? startDate, DateTime? endDate, int priority, Guid createdBy, Guid updatedBy,
        List<User>? users, List<Category>? categories
    )
    {
        return new UserTask(
            Guid.NewGuid(), title, description, deadline,
            startDate, endDate, priority, DateTime.UtcNow, createdBy, DateTime.UtcNow,
            updatedBy, users, categories
        );
    }

    public void UpdateTask(
        string title, string? description, DateTime deadline,
        DateTime? startDate, DateTime? endDate, int priority, Guid updatedBy,
        List<User>? users, List<Category>? categories
    )
    {
        Title = title;
        Description = description;
        Deadline = deadline;
        StartDate = startDate;
        EndDate = endDate;
        Priority = priority;
        LastUpdate = DateTime.UtcNow;
        UpdatedBy = updatedBy;
        Users = users ?? new List<User>();
        Categories = categories ?? new List<Category>();
    }

    public void UpdateUsers(List<User> users)
    {
        Users = new List<User>(users);
    }
}