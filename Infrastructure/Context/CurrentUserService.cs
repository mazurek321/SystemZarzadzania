using Core.Models;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Context;
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? Id
    {
        get
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("userid")?.Value;
            return Guid.TryParse(userId, out var id) ? id : (Guid?)null;
        }
    }

     public User.UserRole? Role
    {
        get
        {
            var roleStr = _httpContextAccessor.HttpContext?.User?.FindFirst("userrole")?.Value;
            if (Enum.TryParse<User.UserRole>(roleStr, out var role))
            {
                return role;
            }
            return null;
        }
    }
}
