using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Database;
using Core.Models.Users;
using Core.Models.Messages;
using Core.Dto;
using Infrastructure.Context;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly ICurrentUserService _user;
    private readonly IUserRepository _userRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly ILogger<MessageController> _logger;
    private readonly IMessageSender _messageSender;

    public MessageController(
        AppDbContext dbContext,
        ICurrentUserService user,
        IUserRepository userRepository,
        IMessageRepository messageRepository,
        ILogger<MessageController> logger,
        IMessageSender messageSender
    )
    {
        _dbContext = dbContext;
        _user = user;
        _userRepository = userRepository;
        _messageRepository = messageRepository;
        _logger = logger;
        _messageSender = messageSender;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateMessage(
        [FromQuery] Guid receiverId,
        [FromBody] string messageContent
    )
    {
        var sender = await _userRepository.FindByIdAsync(_user.Id.Value);
        if (sender is null)
            return NotFound("Error during sending message.");

        var receiver = await _userRepository.CheckByIdIfExistsAsync(receiverId);
        if (!receiver)
            return NotFound("Receiver not found.");

        var newMessage = Message.NewMessage(
            messageContent,
            sender.Id,
            receiverId
        );

        await _messageRepository.AddAsync(newMessage);

        await _messageSender.SendMessageAsync(newMessage);

        _logger.LogInformation("[Create] User {userId} sent message to {receiver} : {message}", sender.Id, receiverId, messageContent);

        return Ok(newMessage);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetMessage([FromQuery] Guid messageId)
    {
        var message = await _messageRepository.GetByIdAsync(messageId);
        if (message is null)
            return NotFound("Message not found.");

        return Ok(message);
    }

    [HttpGet("browse")]
    [Authorize]
    public async Task<ActionResult<PagedResult<Message>>> BrowseMessagesWithUser(
        [FromQuery] Guid receiverId, int pageNumber = 1, int pageSize = 30
    )
    {
        var receiverExists = await _userRepository.CheckByIdIfExistsAsync(receiverId);
        if (!receiverExists)
            return NotFound("Receiver not found.");

        var user = await _userRepository.CheckByIdIfExistsAsync(_user.Id.Value);
        if (!user)
            return NotFound("User not found.");

        var pagedResult = await _messageRepository.BrowseMessagesWithUserAsync(_user.Id.Value, receiverId, pageNumber, pageSize);

        return Ok(pagedResult);
    }
}
