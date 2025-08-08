using Microsoft.AspNetCore.SignalR;
using Core.Models.Messages;
using Core.Models.Chats;
namespace Api.Hubs;

public class MessageHub : Hub
{
    private readonly IChatRepository _chatRepository;
    public MessageHub(
        IChatRepository chatRepository
    )
    {
        _chatRepository = chatRepository;
    }
    public async Task SendMessage(Message message, Guid chatId)
    {
        var chat = await _chatRepository.GetByIdAsync(chatId);
        if (chat == null) return;

        foreach (var participantId in chat.Participants)
        {
            await Clients.User(participantId.ToString())
                         .SendAsync("ReceiveMessage", new
                         {
                             SenderUserId = message.SenderUserId,
                             MessageContent = message.MessageContent,
                             SentAt = message.SentAt,
                             ChatId = chatId
                         });
        }
    }

    public async Task Typing(string chatId)
    {
        var senderUserId = Context.UserIdentifier;
         if (string.IsNullOrEmpty(senderUserId))
                return;
        
        if (!Guid.TryParse(chatId, out var chatGuid))
            return;

        var chat = await _chatRepository.GetByIdAsync(chatGuid);
            if (chat == null)
                return;

       foreach (var participantId in chat.Participants)
            {
                if (participantId.ToString() != senderUserId)
                {
                    await Clients.User(participantId.ToString())
                                 .SendAsync("ReceiveTyping", senderUserId, chatId);
                }
            }
    }
}