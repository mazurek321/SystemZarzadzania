using Core.Models.Messages;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Api.Hubs;
using Core.Models.Chats;

namespace Api.Services;

public class MessageSender : IMessageSender
{
    private readonly IHubContext<MessageHub> _hubContext;
    private readonly IChatRepository _chatRepository;
    public MessageSender(
        IHubContext<MessageHub> hubContext,
        IChatRepository chatRepository
    )
    {
        _hubContext = hubContext;
        _chatRepository = chatRepository;
    }

    public async Task SendMessageAsync(Message message, Guid chatId)
    {
        var chat = await _chatRepository.GetByIdAsync(chatId);
        if (chat is null)
            return;

        foreach (var participantId in chat.Participants)
        {
            await _hubContext.Clients.User(participantId.ToString())
                       .SendAsync("ReceiveMessage", new
                       {
                           SenderUserId = message.SenderUserId,
                           MessageContent = message.MessageContent,
                           SentAt = message.SentAt,
                           ChatId = chat.Id
                       });
        }

    }

}