namespace Core.Models.Users;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User> FindByIdAsync(Guid id);
    Task<User> FindByEmailAsync(string email);
    Task UpdateActivityAsync(User user);
}