using Core.Models.Notifications;
using Infrastructure.Database;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Core.Models.Notifications;
using Core.Models.UserTasks;
using Infrastructure.Context;
using Core.Models.Users;


namespace Infrastructure.Services.Checkers;

public class DeadlineChecker : INotificationChecker
{
    private readonly IServiceProvider _serviceProvider;
    private readonly AppDbContext _dbContext;
    private readonly ILogger<DeadlineChecker> _logger;
    private readonly IUserTaskRepository _userTaskRepository;
    private readonly ICurrentUserService _user;
    private readonly IUserRepository _userRepository;

    public DeadlineChecker(
        IServiceProvider serviceProvider,
        AppDbContext dbContext,
        ILogger<DeadlineChecker> logger,
        IUserTaskRepository userTaskRepository,
        ICurrentUserService user,
        IUserRepository userRepository
    )
    {
        _serviceProvider = serviceProvider;
        _dbContext = dbContext;
        _logger = logger;
        _userTaskRepository = userTaskRepository;
        _user = user;
        _userRepository = userRepository;
    }

    public async Task CheckAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var notificationSender = scope.ServiceProvider.GetRequiredService<INotificationSender>();

        if (!_user.Id.HasValue)
            return;
            
        var user = await _userRepository.FindByIdAsync(_user.Id.Value);
        var time = DateTime.UtcNow.AddDays(1);
        var tasks = await _userTaskRepository.GetWithComingDeadlinesAsync(time, user.Id);

        foreach (var task in tasks)
        {
            var message = $"Coming deadline for task: {task.Title}.";
            await notificationSender.SendNotificationToUserAsync(user.Id, message);
            
            _logger.LogInformation("[NotificationCreate] Sent notification to {UserId}, message: {message}", user.Id, message);
        }
    }
}
