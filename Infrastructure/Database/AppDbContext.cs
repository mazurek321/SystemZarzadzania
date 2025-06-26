using Microsoft.EntityFrameworkCore;
using Core.Models.Users;
using Core.Models.OutboxMessages;
using Core.Models.UserTasks;
using Core.Models.Categories;
using Core.Domain;
using Newtonsoft.Json;
using System.Linq;

namespace Infrastructure.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserTask> Tasks { get; set; }
    public DbSet<Category> Categories { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEntities = ChangeTracker.Entries()
            .Where(x => x.Entity is IDomainEventEntity &&
                        (x.State == EntityState.Added || x.State == EntityState.Modified))
            .Select(x => x.Entity as IDomainEventEntity)
            .Where(x => x != null)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.DomainEvents)
            .ToList();

        foreach (var entity in domainEntities)
        {
            entity.ClearDomainEvents();
        }

        foreach (var domainEvent in domainEvents)
        {
            var typeName = domainEvent.GetType().AssemblyQualifiedName;
            var data = JsonConvert.SerializeObject(domainEvent);
            var outboxMessage = new OutboxMessage(domainEvent.OccurredOn, typeName, data);
            OutboxMessages.Add(outboxMessage);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserTask>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<UserTask>()
            .HasMany(t => t.Categories)
            .WithMany(c => c.Tasks)
            .UsingEntity(j => j.ToTable("TaskCategories"));

        modelBuilder.Entity<User>()
            .HasMany(t => t.Tasks)
            .WithMany(c => c.Users)
            .UsingEntity(j => j.ToTable("UserTasksAssignment"));
    
    }
}
