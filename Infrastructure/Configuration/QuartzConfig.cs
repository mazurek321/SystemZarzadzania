using System;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Infrastructure.Quartz;

namespace Infrastructure.Configuration;

public static class QuartzConfig
{
    public static IServiceCollection AddQuartzJobs(this IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();

            q.ScheduleJob<ProcessOutboxJob>(trigger => trigger
                .WithIdentity("ProcessOutboxJob-trigger")
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(30).RepeatForever()));

            q.ScheduleJob<NotificationCheckJob>(trigger => trigger
                .WithIdentity("NotificationCheckJob-trigger")
                .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromMinutes(10)).RepeatForever()));
        });

        services.AddQuartzHostedService(options => 
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }
}
