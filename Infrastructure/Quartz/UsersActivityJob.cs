using Quartz;
using Microsoft.Extensions.Logging;
using Core.Models.Users;

namespace Infrastructure.Quartz;

[DisallowConcurrentExecution]
public class UsersActivityJob : IJob
{
    private readonly ILogger<UsersActivityJob> _logger;
    private readonly IUserRepository _userRepository;

    public UsersActivityJob(
        ILogger<UsersActivityJob> logger,
        IUserRepository userRepository
    )
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Running UsersActivityJob at {Time}", DateTime.UtcNow);

        try
        {
            var users = await _userRepository.GetActiveUsers();

            foreach (var user in users)
            {
                if (user.LastActive <= DateTime.UtcNow.AddMinutes(-15))
                {
                    await _userRepository.UpdateActivityAsync(user, false);
                    _logger.LogInformation("Set user " + user.Email + " as inactive.");
                }
            }
        }
        catch (Exception err)
        {
            _logger.LogError("Error: ", err);
        }
    }
}
