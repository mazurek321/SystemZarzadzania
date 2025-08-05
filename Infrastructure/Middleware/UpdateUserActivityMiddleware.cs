using Microsoft.AspNetCore.Http;
using Core.Models.Users;
using System.Threading.Tasks;

namespace Infrastructure.Middleware;

public class UpdateUserActivityMiddleware
{
    private readonly RequestDelegate _next;
    public UpdateUserActivityMiddleware(
        RequestDelegate next
    )
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserRepository _userRepository)
    {
        if (context.User.Identity.IsAuthenticated)
        {
            var userIdString = context.User.FindFirst("userid")?.Value;
            if (Guid.TryParse(userIdString, out var userId))
            {
                var user = await _userRepository.FindByIdAsync(userId);
                if (user != null)
                {
                    await _userRepository.UpdateActivityAsync(user, true);
                }
            }
        }

        await _next(context);
    }
}