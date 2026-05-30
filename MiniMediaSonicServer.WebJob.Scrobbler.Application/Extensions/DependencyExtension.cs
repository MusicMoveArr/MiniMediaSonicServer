using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MiniMediaSonicServer.WebJob.Scrobbler.Application.Handlers.Scrobblers;
using MiniMediaSonicServer.WebJob.Scrobbler.Application.Jobs;
using MiniMediaSonicServer.WebJob.Scrobbler.Application.Services;
using Quartz;

namespace MiniMediaSonicServer.WebJob.Scrobbler.Application.Extensions;


public static class DependencyExtension
{
    public static IServiceCollection AddScrobblerDependencies(this IServiceCollection services) =>
        services.AddScoped<ScrobblerJob>()
            .AddScoped<ScrobblerService>()
            .AddScoped<ListenBrainzScrobbleHandler>()
            .AddScoped<MalojaScrobbleHandler>()
            .AddScoped<LibreFmScrobbleHandler>();
    
    public static IServiceCollectionQuartzConfigurator AddScrobblerJobs(
        this IServiceCollectionQuartzConfigurator config,
        WebApplicationBuilder builder)
    {
        var jobKey = new JobKey("ScrobblerJob");
        config.AddJob<ScrobblerJob>(opts => opts.WithIdentity(jobKey));
        config.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity("ScrobblerJob-trigger")
            .WithCronSchedule(builder.Configuration.GetSection("Jobs")["ScrobblerCron"]));
        
        
        return config;
    }
}