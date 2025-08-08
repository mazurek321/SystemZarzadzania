using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Database;
using Core.Models.Users;
using Core.Models.Messages;
using Core.Models.Chats;
using Core.Dto;
using Api.Dto.ChatDto;
using Infrastructure.Context;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly ICurrentUserService _user;
    private readonly IUserRepository _userRepository;
    private readonly IChatRepository _chatRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly ILogger<ChatController> _logger;

    public ChatController(
        AppDbContext dbContext,
        ICurrentUserService user,
        IUserRepository userRepository,
        IChatRepository chatRepository,
        IMessageRepository messageRepository,
        ILogger<ChatController> logger
    )
    {
        _dbContext = dbContext;
        _user = user;
        _userRepository = userRepository;
        _chatRepository = chatRepository;
        _messageRepository = messageRepository;
        _logger = logger;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateChat(
        [FromBody] CreateChatDto dto
    )
    {
        var chat = Chat.NewChat(
            dto.Name,
            dto.Participants
        );

        await _chatRepository.AddChatAsync(chat);
        return Ok(chat);
    }

    [HttpPost("add/user")]
    [Authorize]
    public async Task<IActionResult> AddParticipant(
        [FromQuery] Guid chatId,
        [FromBody] Guid participantId
    )
    {
        var participant = await _userRepository.FindByIdAsync(participantId);
        if (participant is null)
            return NotFound("Participant not found.");

        var chat = await _chatRepository.GetByIdAsync(chatId);
        if (chat is null)
            return NotFound("Chat not found.");

        chat.AddParticipant(participantId);
        await _chatRepository.UpdateAsync(chat);    

        return Ok(chat);
    }
    

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetChat([FromQuery] Guid chatId)
    {
        var chat = await _chatRepository.GetByIdAsync(chatId);
        if (chat is null)
            return NotFound("Chat not found.");

        return Ok(chat);
    }

    [HttpGet("browse")]
    [Authorize]
    public async Task<ActionResult> BrowseChats(
        [FromQuery] int pageNumber = 1, int pageSize = 10
    )
    {
        var user = await _userRepository.FindByIdAsync(_user.Id.Value);
        if (user is null)
            return NotFound("User not found.");

        var chats = await _chatRepository.BrowseChats(user.Id, pageNumber, pageSize);
        return Ok(chats);
    }

}
