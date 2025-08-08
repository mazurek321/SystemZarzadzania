
namespace Core.Models.Messages;
public interface IMessageSender
{
    Task SendMessageAsync(Message message, Guid chatId);
}