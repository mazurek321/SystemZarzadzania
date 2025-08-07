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
    public async Task<PagedResult<Message>> BrowseMessagesWithUserAsync(Guid userId, Guid secondUserId, int pageNumber, int pageSize)
    {
        var allMessages = dbContext.Messages
                                    .AsNoTracking()
                                    .Where(x =>
                                        (x.SenderUserId == userId || x.ReceiverUserId == userId)
                                        &&
                                        (x.SenderUserId == secondUserId || x.ReceiverUserId == secondUserId)
                                    )
                                    .OrderBy(x => x.SentAt);

        var count = await allMessages.CountAsync();

        var messages = await allMessages
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();


        return new PagedResult<Message>
        {
            Items = messages,
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