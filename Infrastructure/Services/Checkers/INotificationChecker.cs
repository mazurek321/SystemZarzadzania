namespace Infrastructure.Services.Checkers;
public interface INotificationChecker
{
    Task CheckAsync(CancellationToken cancellationToken);
}
