
using Infrastructure.Services.Checkers;
using Infrastructure.Email;
using Infrastructure.Email.EmailTypes;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services;
public static class Extensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {

        services.AddScoped<INotificationChecker, DeadlineChecker>();
        
        services.AddScoped<IEmailTemplate, Registration>();
        services.AddScoped<EmailTemplateFactory>();
        services.AddScoped<EmailSender>();
        services.AddHostedService<Infrastructure.Services.NotificationService>();

        return services;
    }
}