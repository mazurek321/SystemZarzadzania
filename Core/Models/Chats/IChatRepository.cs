using Core.Dto;
namespace Core.Models.Chats;

public interface IChatRepository
{
    Task AddChatAsync(Chat chat);
    Task<Chat> GetByIdAsync(Guid chatId);
    Task<PagedResult<Chat>> BrowseChats(Guid userId, int pageNumber, int pageSize);
    Task UpdateAsync(Chat chat);
    Task DeleteAsync(Chat chat);
}