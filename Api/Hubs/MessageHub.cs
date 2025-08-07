using Microsoft.AspNetCore.SignalR;
using Core.Models.Messages;
namespace Api.Hubs;

public class MessageHub : Hub
{
    public async Task SendMessage(string senderId, string receiverId, string messageContent, DateTime sentAt)
    {
        await Clients.User(receiverId).SendAsync("ReceiveMessage", new
        {
            SenderUserId = senderId,
            MessageContent = messageContent,
            SentAt = sentAt
        });
    }

    public async Task Typing(string receiverUserId)
    {
        var senderUserId = Context.UserIdentifier;

        if(senderUserId is not null)
            await Clients.User(receiverUserId).SendAsync("ReceiveTyping", senderUserId);
    }
}