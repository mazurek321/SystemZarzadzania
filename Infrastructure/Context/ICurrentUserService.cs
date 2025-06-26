using Core.Models.Users;

namespace Infrastructure.Context;
public interface ICurrentUserService
{
    Guid? Id { get; }
    User.UserRole? Role { get; }
}