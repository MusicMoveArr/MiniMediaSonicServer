using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MiniMediaSonicServer.WebJob.Playlists.Application.Jobs;
using MiniMediaSonicServer.WebJob.Playlists.Application.Repositories;
using MiniMediaSonicServer.WebJob.Playlists.Application.Services;
using Quartz;

namespace MiniMediaSonicServer.WebJob.Playlists.Application.Extensions;

public static class DependencyExtension
{
    public static IServiceCollection AddPlaylistDependencies(this IServiceCollection services) =>
        services.AddScoped<PlaylistImportService>()
            .AddScoped<PlaylistImportRepository>();
    
    public static IServiceCollectionQuartzConfigurator AddPlaylistJobs(
        this IServiceCollectionQuartzConfigurator config,
        WebApplicationBuilder builder)
    {
        var jobKey = new JobKey("PlaylistImportJob");

        config.AddJob<PlaylistImportJob>(opts => opts.WithIdentity(jobKey));

        config.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity("PlaylistImportJob-trigger")
            .WithCronSchedule(builder.Configuration.GetSection("Jobs")["PlaylistImportCron"]));
        return config;
    }
}