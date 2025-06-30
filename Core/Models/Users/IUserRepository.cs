namespace Core.Models.Users;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User> FindByIdAsync(Guid id);
    Task<List<User>> FindByIdsAsync(List<Guid> list);
    Task<User> FindByEmailAsync(string email);
    Task<List<User>> BrowseUsers(int pageNumber, int pageSize);
    Task UpdateActivityAsync(User user);
    Task UpdateInformationAsync(User user);
    Task DeleteUserAsync(User user);

}