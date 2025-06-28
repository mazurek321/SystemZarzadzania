using Core.Models.Categories;
using Core.Models.Users;

namespace Core.Models.UserTasks;

public class UserTask
{
    public UserTask() { }
    public UserTask(Guid id, string title, string description,
                    DateTime startDate, DateTime endDate, int priority,
                    DateTime createdAt, Guid createdBy,
                    User createdByUser, DateTime lastUpdate,
                    Guid updatedBy, User updatedByUser,
                    List<User> users, List<Category> categories)
    {
        Id = id;
        Title = title;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        Priority = priority;
        CreatedAt = createdAt;
        CreatedBy = createdBy;
        CreatedByUser = createdByUser;
        LastUpdate = lastUpdate;
        UpdatedBy = updatedBy;
        UpdatedByUser = updatedByUser;
        Users = users;
        Categories = categories;
    }
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int Priority { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public User CreatedByUser { get;  private set; }
    public DateTime LastUpdate { get; private set; }
    public Guid UpdatedBy { get; private set; }
    public User UpdatedByUser { get; private set; }
    public ICollection<User> Users { get; private set; }
    public List<Category> Categories { get; private set; }

    public static UserTask NewTask(
        string title, string description,
        DateTime startDate, DateTime endDate, int priority, DateTime createdAt, Guid createdBy,
        User createdByUser, DateTime lastUpdate, Guid updatedBy, User updatedByUser,
        List<User> users, List<Category> categories
    )
    {
        return new UserTask(
            Guid.NewGuid(), title, description,
            startDate, endDate, priority, createdAt, createdBy, createdByUser, lastUpdate,
            updatedBy, updatedByUser, users, categories
        );
    }

    public void UpdateTask(
        string title, string description,
        DateTime startDate, DateTime endDate, int priority,
        DateTime lastUpdate, Guid updatedBy, User updatedByUser,
        List<User> users, List<Category> categories
    )
    {
        Title = title;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        Priority = priority;
        LastUpdate = lastUpdate;
        UpdatedBy = updatedBy;
        UpdatedByUser = updatedByUser;
        Users = users;
        Categories = categories;
    }
}