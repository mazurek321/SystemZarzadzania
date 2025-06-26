using Microsoft.EntityFrameworkCore;
using Core.Models.Users;
using Core.Models.OutboxMessages;
using Core.Models.UserTasks;
using Core.Domain;
using Newtonsoft.Json;



namespace Infrastructure.Database;

public class AppDbContext : DbContext
{

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }


    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserTask> Tasks { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEntities = ChangeTracker.Entries()
            .Where(x => x.Entity is IDomainEventEntity && x.State == EntityState.Added || x.State == EntityState.Modified);

        var domainEvents = domainEntities
            .SelectMany(x => ((IDomainEventEntity)x.Entity).DomainEvents)
            .ToList();

        foreach (var entity in domainEntities)
        {
            ((IDomainEventEntity)entity.Entity).ClearDomainEvents();
        }

        foreach (var domainEvent in domainEvents)
        {
            var typeName = domainEvent.GetType().AssemblyQualifiedName;
            var data = JsonConvert.SerializeObject(domainEvent);

            var outboxMessage = new OutboxMessage(domainEvent.OccurredOn, typeName, data);
            OutboxMessages.Add(outboxMessage);
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }



}