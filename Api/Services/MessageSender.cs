using Core.Models.Messages;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Api.Hubs;

namespace Api.Services;

public class MessageSender : IMessageSender
{
    private readonly IHubContext<MessageHub> _hubContext;
    public MessageSender(
        IHubContext<MessageHub> hubContext
    )
    {
        _hubContext = hubContext;
    }

    public async Task SendMessageAsync(Message message)
    {
        await _hubContext.Clients.User(message.ReceiverUserId.ToString())
            .SendAsync("ReceiveMessage", new
            {
                SenderUserId = message.SenderUserId,
                MessageContent = message.MessageContent,
                SentAt = message.SentAt
            });
    }

}