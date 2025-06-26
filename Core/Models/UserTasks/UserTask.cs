using Core.Models.Categories;
using Core.Models.Users;

namespace Core.Models.UserTasks;

public class UserTask
{
    public UserTask() { }
    public UserTask(Guid id, string title, string description,
                    DateTime startDate, DateTime endDate, int priority,
                    List<User> users, List<Category> categories)
    {
        Id = id;
        Title = title;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        Priority = priority;
        Users = users;
        Categories = categories;
    }
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int Priority { get; private set; }
    public ICollection<User> Users { get; private set; }
    public List<Category> Categories { get; private set; }

    public static UserTask NewTask(
        string title, string description,
        DateTime startDate, DateTime endDate, int priority, List<User> users,
        List<Category> categories
    )
    {
        return new UserTask(Guid.NewGuid(), title, description,
            startDate, endDate, priority, users, categories
        );
    }

    public void UpdateTask(string title, string description,
        DateTime startDate, DateTime endDate, int priority, List<User> users, List<Category> categories
    )
    {
        Title = title;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        Priority = priority;
        Users = users;
        Categories = categories;
    }
}