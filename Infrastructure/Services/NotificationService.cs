using System;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Database;
using Core.Models.Notifications;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Services.Checkers;


namespace Infrastructure.Services;
public class NotificationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly AppDbContext _dbContext;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IServiceProvider serviceProvider,
        AppDbContext dbContext,
        ILogger<NotificationService> logger)
    {
        _serviceProvider = serviceProvider;
        _dbContext = dbContext;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var checkers = scope.ServiceProvider.GetServices<INotificationChecker>();

            foreach (var checker in checkers)
            {
                try
                {
                    await checker.CheckAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during notification check.");
                }
            }
            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        }
    }
}
