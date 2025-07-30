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

        foreach (var task in uncompletedTasks)
        {
            if (task.Deadline <= DateTime.UtcNow)
            {
                _logger.LogWarning($"Task {task.Id} missed deadline");
                task.UpdateStatus(UserTask.TaskStatus.Overdue);
                await _userTaskRepository.UpdateAsync(task);

                await NotifyUsers(task, NotificationType.Alert, $"Task {task.Title} missed deadline");
            }

            else if (task.Deadline <= DateTime.UtcNow.AddDays(1))
            {
                _logger.LogWarning($"Task {task.Id} will soon miss deadline");
                task.UpdateStatus(UserTask.TaskStatus.Almostdue);
                await _userTaskRepository.UpdateAsync(task);

                await NotifyUsers(task, NotificationType.Warning, $"Task {task.Title} will soon miss deadline.");
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