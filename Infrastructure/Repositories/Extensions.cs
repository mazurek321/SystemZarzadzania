using Core.Models.Users;
using Core.Models.Tokens;
using Core.Models.UserTasks;
using Infrastructure.Services;
using Infrastructure.Context;

using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Repositories;
public static class Extensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserTaskRepository, UserTaskRepository>();


        return services;
    }
}