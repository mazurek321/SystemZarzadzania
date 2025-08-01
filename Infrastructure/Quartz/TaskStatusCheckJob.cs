using Quartz;
using Microsoft.Extensions.Logging;
using Core.Models.UserTasks;
using Core.Models.Notifications;

namespace Infrastructure.Quartz;

[DisallowConcurrentExecution]
public class TaskStatusCheckJob : IJob
{
    private readonly ILogger<TaskStatusCheckJob> _logger;
    private readonly IUserTaskRepository _userTaskRepository;
    private readonly INotificationSender _notificationSender;

    public TaskStatusCheckJob(
        ILogger<TaskStatusCheckJob> logger,
        IUserTaskRepository userTaskRepository,
        INotificationSender notificationSender
    )
    {
        _logger = logger;
        _userTaskRepository = userTaskRepository;
        _notificationSender = notificationSender;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Running TaskStatusCheckJob at {Time}", DateTime.UtcNow);

        var uncompletedTasks = await _userTaskRepository.GetUncompletedTasksAsync();

        var notifyTasks = new List<Task>();

        foreach (var task in uncompletedTasks)
        {
            string? message = null;
            var statusChanged = false;
            NotificationType? notifType = null;

            if (task.Deadline <= DateTime.UtcNow)
            {
                if (task.Status != UserTask.TaskStatus.Overdue)
                {

                    task.UpdateStatus(UserTask.TaskStatus.Overdue);

                    statusChanged = true;
                    notifType = NotificationType.Alert;
                    message = $"Task '{task.Title}' missed the deadline.";

                    _logger.LogWarning(message);
                }

            }

            else if (task.Deadline <= DateTime.UtcNow.AddDays(1))
            {
                if (task.Status != UserTask.TaskStatus.Almostdue)
                {

                    task.UpdateStatus(UserTask.TaskStatus.Almostdue);

                    statusChanged = true;
                    notifType = NotificationType.Warning;
                    message = $"Task '{task.Title}' will soon miss the deadline.";

                    _logger.LogWarning(message);
                }
            }

            if (statusChanged)
            {
                await _userTaskRepository.UpdateAsync(task);
                await NotifyUsers(task, notifType!.Value, message!);
            }
        }
    }

    private async Task NotifyUsers(UserTask task, NotificationType type, string message)
    {
        await _notificationSender.SendNotificationToUserAsync(task.CreatedBy, type, message);

        var usersToNotify = task.Users.Select(u => u.Id);
        foreach (var userId in usersToNotify)
        {
            await _notificationSender.SendNotificationToUserAsync(userId, type, message);
        }
    }
}