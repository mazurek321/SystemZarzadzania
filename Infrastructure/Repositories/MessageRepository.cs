using Core.Models.Messages;
using Core.Dto;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal sealed class MessageRepository(AppDbContext dbContext) : IMessageRepository
{
    public async Task AddAsync(Message message)
    {
        dbContext.Messages.Add(message);
        await dbContext.SaveChangesAsync();
    }
    public async Task<Message> GetByIdAsync(Guid messageId)
    {
        return await dbContext.Messages.FirstOrDefaultAsync(x=>x.Id == messageId);
    }
    public async Task<PagedResult<Message>> BrowseMessages(Guid chatId, int pageNumber, int pageSize)
    {
        var chat = await dbContext.Chats
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == chatId);
        
        var messagesQuery = dbContext.Messages
                                        .AsNoTracking()
                                        .Where(m => chat.Messages.Contains(m.Id))
                                        .OrderBy(m => m.SentAt);

        var count = await messagesQuery.CountAsync();

        var messages = await messagesQuery
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();


        return new PagedResult<Message>
        {
            Items = messages,
            TotalCount = count
        };
    }

    public async Task<PagedResult<Message>> BrowseUnreadMessages(Guid chatId, Guid userId)
    {
        var chat = await dbContext.Chats
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == chatId);

        var query = dbContext.Messages
            .AsNoTracking()
            .Where(m => chat.Messages.Contains(m.Id) &&
                        m.Statuses.Any(s => s.UserId == userId && !s.IsRead))
            .OrderBy(m => m.SentAt);

        var count = await query.CountAsync();
        var items = await query.ToListAsync();

        return new PagedResult<Message>
        {
            Items = items,
            TotalCount = count
        };
    }

    public async Task UpdateAsync(Message message)
    {
        dbContext.Messages.Update(message);
        await dbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Message message)
    {
        dbContext.Messages.Remove(message);
        await dbContext.SaveChangesAsync();
    }

}