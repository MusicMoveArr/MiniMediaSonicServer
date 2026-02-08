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
            .AddScoped<PlaylistImportRepository>()
            .AddScoped<FixMissingPlaylistTracksService>()
            .AddScoped<FixMissingPlaylistTracksRepository>();
    
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
        
        
        var fixTracksjobKey = new JobKey("FixMissingPlaylistTracksJob");
        config.AddJob<FixMissingPlaylistTracksJob>(opts => opts.WithIdentity(fixTracksjobKey));
        config.AddTrigger(opts => opts
            .ForJob(fixTracksjobKey)
            .WithIdentity("FixMissingPlaylistTracksJob-trigger")
            .WithCronSchedule(builder.Configuration.GetSection("Jobs")["PlaylistFixTracksCron"]));
        return config;
    }
}