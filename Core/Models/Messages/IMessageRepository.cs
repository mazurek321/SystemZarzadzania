using Core.Dto;
namespace Core.Models.Messages;

public interface IMessageRepository
{
    Task AddAsync(Message message);
    Task<Message> GetByIdAsync(Guid messageId);
    Task<PagedResult<Message>> BrowseMessages(Guid chatId, int pageNumber, int pageSize);
    Task<PagedResult<Message>> BrowseUnreadMessages(Guid chatId,Guid userId);
    Task UpdateAsync(Message message);
    Task DeleteAsync(Message message);
}