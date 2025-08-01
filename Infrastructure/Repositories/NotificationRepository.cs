using Core.Models.Notifications;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal sealed class NotificationRepository(AppDbContext dbContext) : INotificationRepository
{
    public async Task AddAsync(Notification notif)
    {
        dbContext.Notifications.Add(notif);
        await dbContext.SaveChangesAsync();
    }

    public async Task<List<Notification>> BrowseNotifications(Guid? userId, bool? isRead)
    {
        var query = dbContext.Notifications.AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(x => x.UserId == userId.Value);
        }

        if (isRead.HasValue)
        {
            query = query.Where(x => x.IsRead == isRead);
        }

        return await query.OrderByDescending(x=>x.ReceivedAt).ToListAsync();
    }

    public async Task<Notification> GetNotification(Guid notificationId)
    {
        return await dbContext.Notifications.FirstOrDefaultAsync(x => x.Id == notificationId);
    }

    public async Task<bool> CheckIfAlreadyExistsAsync(Guid userId, string message)
    {
        return await dbContext.Notifications
                                        .AnyAsync(x => x.UserId == userId && x.Message == message);
    }

    public async Task UpdateAsync(Notification notif)
    {
        dbContext.Notifications.Update(notif);
        await dbContext.SaveChangesAsync();
    }
}