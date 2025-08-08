using Core.Models.Chats;
using Core.Dto;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal sealed class ChatRepository(AppDbContext dbContext) : IChatRepository
{

    public async Task AddChatAsync(Chat chat)
    {
        dbContext.Chats.Add(chat);
        await dbContext.SaveChangesAsync();
    }
    public async Task<Chat> GetByIdAsync(Guid chatId)
    {
        return await dbContext.Chats.FirstOrDefaultAsync(x=>x.Id == chatId);
    }
    public async Task<PagedResult<Chat>> BrowseChats(Guid userId, int pageNumber, int pageSize)
    {
         var query = dbContext.Chats
                                .AsNoTracking()
                                .Where(c => c.Participants.Contains(userId))
                                .OrderByDescending(c => dbContext.Messages
                                    .Where(m => c.Messages.Contains(m.Id) 
                                                && m.Statuses.Any(s => s.UserId == userId && !s.IsRead))
                                    .Any())
                                .ThenByDescending(c => dbContext.Messages
                                    .Where(m => c.Messages.Contains(m.Id))
                                    .Max(m => (DateTime?)m.SentAt)); 

            var count = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Chat>
            {
                Items = items,
                TotalCount = count
            };
    }

    public async Task UpdateAsync(Chat chat)
    {
        dbContext.Chats.Update(chat);
        await dbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Chat chat)
    {
        dbContext.Chats.Remove(chat);
        await dbContext.SaveChangesAsync();
    }

}