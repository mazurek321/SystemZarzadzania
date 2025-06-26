namespace Core.Models.Users;
public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User> FindByEmailAsync(string email);
}