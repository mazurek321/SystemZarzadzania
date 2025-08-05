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
        INotificationSender notificationSender)
    {
        _logger = logger;
        _userTaskRepository = userTaskRepository;
        _notificationSender = notificationSender;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Running TaskStatusCheckJob at {Time}", DateTime.UtcNow);

        var uncompletedTasks = await _userTaskRepository.GetUncompletedTasksAsync();

        foreach (var task in uncompletedTasks)
        {
            var oldStatus = task.Status;

            task.RefreshStatus();

            if (task.Status != oldStatus)
            {
                string message = $"Task '{task.Title}' changed status from {oldStatus} to {task.Status}.";
                NotificationType notificationType = GetNotificationType(task.Status);

                _logger.LogInformation(message);

                await _userTaskRepository.UpdateAsync(task);
                await NotifyUsers(task, notificationType, message);
            }
        }
    }

    private NotificationType GetNotificationType(UserTask.TaskStatus status)
    {
        return status switch
        {
            UserTask.TaskStatus.Almostdue => NotificationType.Warning,
            UserTask.TaskStatus.Overdue or UserTask.TaskStatus.InProgressOverdue => NotificationType.Alert,
            _ => NotificationType.Normal
        };
    }

    private async Task NotifyUsers(UserTask task, NotificationType type, string message)
    {
        await _notificationSender.SendNotificationToUserAsync(task.CreatedBy, type, message);

        foreach (var userId in task.Users.Select(u => u.Id))
        {
            await _notificationSender.SendNotificationToUserAsync(userId, type, message);
        }
    }
}
