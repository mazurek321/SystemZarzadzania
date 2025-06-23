using Core.Models;

namespace Infrastructure.Context;
public interface ICurrentUserService
{
    Guid? Id { get; }
    User.UserRole? Role { get; }
}