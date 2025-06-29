using Core.Models.Categories;
using Core.Models.Users;

namespace Core.Models.UserTasks;

public class UserTask
{
    public UserTask() { }
    public UserTask(Guid id, string title, string description,
                    DateTime startDate, DateTime endDate, int priority,
                    DateTime createdAt, Guid createdBy,
                    DateTime lastUpdate,Guid updatedBy,
                    List<Guid> users, List<int> categories)
    {
        Id = id;
        Title = title;
        Description = description;
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
    public string Description { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int Priority { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime LastUpdate { get; private set; }
    public Guid UpdatedBy { get; private set; }
    public ICollection<Guid> Users { get; private set; }
    public List<int> Categories { get; private set; }

    public static UserTask NewTask(
        string title, string description,
        DateTime startDate, DateTime endDate, int priority, DateTime createdAt, Guid createdBy,
         DateTime lastUpdate, Guid updatedBy,
        List<Guid> users, List<int> categories
    )
    {
        return new UserTask(
            Guid.NewGuid(), title, description,
            startDate, endDate, priority, createdAt, createdBy, lastUpdate,
            updatedBy, users, categories
        );
    }

    public void UpdateTask(
        string title, string description,
        DateTime startDate, DateTime endDate, int priority,
        DateTime lastUpdate, Guid updatedBy,
        List<Guid> users, List<int> categories
    )
    {
        Title = title;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        Priority = priority;
        LastUpdate = lastUpdate;
        UpdatedBy = updatedBy;
        Users = users;
        Categories = categories;
    }

    public void UpdateUsers(List<Guid> users)
    {
        Users = new List<Guid>(users);
    }
}