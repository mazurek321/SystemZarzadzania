using System;
using System.Threading.Tasks;
using Quartz;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Infrastructure.Services.Checkers;

namespace Infrastructure.Quartz;

[DisallowConcurrentExecution]
public class NotificationCheckJob : IJob
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationCheckJob> _logger;

    public NotificationCheckJob(IServiceProvider serviceProvider, ILogger<NotificationCheckJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using var scope = _serviceProvider.CreateScope();
        var checkers = scope.ServiceProvider.GetServices<INotificationChecker>();

        foreach (var checker in checkers)
        {
            try
            {
                await checker.CheckAsync(context.CancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during notification check.");
            }
        }
    }
}
