using Core.Dto;
namespace Core.Models.Messages;

public interface IMessageRepository
{
    Task AddAsync(Message message);
    Task<Message> GetByIdAsync(Guid messageId);
    Task<PagedResult<Message>> BrowseMessagesWithUserAsync(Guid userId, Guid secondUserId, int pageNumber, int pageSize);
    Task UpdateAsync(Message message);
    Task DeleteAsync(Message message);
}