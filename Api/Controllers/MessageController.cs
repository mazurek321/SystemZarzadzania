using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Database;
using Core.Models.Users;
using Core.Models.Messages;
using Core.Models.Chats;
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
    private readonly IChatRepository _chatRepository;
    private readonly ILogger<MessageController> _logger;
    private readonly IMessageSender _messageSender;

    public MessageController(
        AppDbContext dbContext,
        ICurrentUserService user,
        IUserRepository userRepository,
        IMessageRepository messageRepository,
        IChatRepository chatRepository,
        ILogger<MessageController> logger,
        IMessageSender messageSender
    )
    {
        _dbContext = dbContext;
        _user = user;
        _userRepository = userRepository;
        _messageRepository = messageRepository;
        _chatRepository = chatRepository;
        _logger = logger;
        _messageSender = messageSender;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateMessage(
        [FromQuery] Guid chatId,
        [FromBody] string messageContent
    )
    {
        var sender = await _userRepository.FindByIdAsync(_user.Id.Value);
        if (sender is null)
            return NotFound("Error during sending message.");

        var chat = await _chatRepository.GetByIdAsync(chatId);
        if (chat is null)
            return NotFound("Chat not found.");

        if (!chat.Participants.Contains(_user.Id.Value))
            return BadRequest("You cannot send message to this chat.");

        var newMessage = Message.NewMessage(
            messageContent,
            sender.Id
        );

        await _messageRepository.AddAsync(newMessage);

        await _messageSender.SendMessageAsync(newMessage, chat.Id);

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
    public async Task<ActionResult<PagedResult<Message>>> BrowseMessages(
        [FromQuery] Guid chatId, int pageNumber = 1, int pageSize = 30
    )
    {
        var chat = await _chatRepository.GetByIdAsync(chatId);
            if (chat == null)
                return NotFound("Chat not found.");

        var user = await _userRepository.CheckByIdIfExistsAsync(_user.Id.Value);
        if (!user)
            return NotFound("User not found.");

        var pagedResult = await _messageRepository.BrowseMessages(chat.Id, pageNumber, pageSize);

        return Ok(pagedResult);
    }
}
