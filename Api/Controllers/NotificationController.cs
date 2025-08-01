using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Database;
using Core.Models.Notifications;
using Core.Models.Users;
using Infrastructure.Context;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Api.Dto.NotificationDto;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IUserRepository _userRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly ICurrentUserService _user;
    private readonly ILogger<NotificationController> _logger;
    public NotificationController(
        AppDbContext dbContext,
        ICurrentUserService user,
        INotificationRepository notificationRepository,
        IUserRepository userRepository,
        ILogger<NotificationController> logger
    )
    {
        _dbContext = dbContext;
        _user = user;
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<NotificationDto>>> BrowseNotifications(
        [FromQuery] Guid? userId, bool? isRead
    )
    {        
        if (userId.HasValue)
        {
            var user = await _userRepository.FindByIdAsync(_user.Id.Value);
            if (user is null)
                return NotFound("User not found.");
        }

        var notifications = await _notificationRepository.BrowseNotifications(userId, isRead);

        var notifDto = notifications.Select(x => new NotificationDto
        {
            Id = x.Id,
            Type = x.Type.ToString(),
            UserId = x.UserId,
            Message = x.Message,
            IsRead = x.IsRead,
            ReadAt = x.ReadAt,
            ReceivedAt = x.ReceivedAt
        }).ToList();

        return Ok(notifDto);
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<NotificationDto>> MarkAsRead(
        [FromBody] List<Guid> notificationIds
    )
    {
        var user = await _userRepository.FindByIdAsync(_user.Id.Value);

        if (user is null)
            return NotFound("User not found.");

        foreach (var notifId in notificationIds)
        {
            var notification = await _notificationRepository.GetNotification(notifId);

            if (notification is null)
                return NotFound("Notification not found.");

            notification.MarkAsRead();
            await _notificationRepository.UpdateAsync(notification);

            Console.WriteLine("Notification UPDATE: ", notification);
        }

        return Ok();
    }


}