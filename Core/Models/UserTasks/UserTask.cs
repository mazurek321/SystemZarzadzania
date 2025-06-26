namespace Core.Models.UserTasks;

public class UserTask
{
    public UserTask() { }
    public UserTask(Guid id, string title, string description, DateTime plannedBegin, DateTime plannedEnd,
                DateTime actualBegin, DateTime actualEnd, int priority, string category)
    {
        Id = id;
        Title = title;
        Description = description;
        PlannedBegin = plannedBegin;
        PlannedEnd = plannedEnd;
        ActualBegin = actualBegin;
        ActualEnd = actualEnd;
        Priority = priority;
        Category = category;
    }
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTime PlannedBegin { get; private set; }
    public DateTime PlannedEnd { get; private set; }
    public DateTime ActualBegin { get; private set; }
    public DateTime ActualEnd { get; private set; }
    public int Priority { get; private set; }
    public string Category { get; private set; }

    public static UserTask NewTask(
        string title, string description, DateTime plannedBegin, DateTime plannedEnd,
        DateTime actualBegin, DateTime actualEnd, int priority, string category
    )
    {
        return new UserTask(Guid.NewGuid(), title, description, plannedBegin, plannedEnd,
            actualBegin, actualEnd, priority, category
        );
    }

    public void UpdateTask(string title, string description, DateTime plannedBegin, DateTime plannedEnd,
        DateTime actualBegin, DateTime actualEnd, int priority, string category
    )
    {
        Title = title;
        Description = description;
        PlannedBegin = plannedBegin;
        PlannedEnd = plannedEnd;
        ActualBegin = actualBegin;
        ActualEnd = actualEnd;
        Priority = priority;
        Category = category;
    }
}