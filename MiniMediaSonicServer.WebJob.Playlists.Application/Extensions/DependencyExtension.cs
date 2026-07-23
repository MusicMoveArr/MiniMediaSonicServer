using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MiniMediaSonicServer.Application.Configurations;
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
            .AddScoped<FixMissingPlaylistTracksRepository>()
            .AddScoped<NavidromeSmartPlaylistService>()
            .AddScoped<NavidromeSmartPlaylistRepository>()
            .AddScoped<NavidromeSmartPlaylistRefreshJob>();
    
    public static IServiceCollectionQuartzConfigurator AddPlaylistJobs(
        this IServiceCollectionQuartzConfigurator config,
        CronConfiguration cronConfig)
    {
        if (!string.IsNullOrWhiteSpace(cronConfig.PlaylistImportCron))
        {
            var jobKey = new JobKey("PlaylistImportJob");
            config.AddJob<PlaylistImportJob>(opts => opts.WithIdentity(jobKey));
            config.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("PlaylistImportJob-trigger")
                .WithCronSchedule(cronConfig.PlaylistImportCron));
        }

        if (!string.IsNullOrWhiteSpace(cronConfig.PlaylistFixTracksCron))
        {
            var fixTracksjobKey = new JobKey("FixMissingPlaylistTracksJob");
            config.AddJob<FixMissingPlaylistTracksJob>(opts => opts.WithIdentity(fixTracksjobKey));
            config.AddTrigger(opts => opts
                .ForJob(fixTracksjobKey)
                .WithIdentity("FixMissingPlaylistTracksJob-trigger")
                .WithCronSchedule(cronConfig.PlaylistFixTracksCron));
        }
        
        if (!string.IsNullOrWhiteSpace(cronConfig.NavidromeSmartPlaylistRefreshCron))
        {
            var refreshNavidromeSmartPlaylistjobKey = new JobKey("NavidromeSmartPlaylistRefreshJob");
            config.AddJob<NavidromeSmartPlaylistRefreshJob>(opts => opts.WithIdentity(refreshNavidromeSmartPlaylistjobKey));
            config.AddTrigger(opts => opts
                .ForJob(refreshNavidromeSmartPlaylistjobKey)
                .WithIdentity("NavidromeSmartPlaylistRefreshJob-trigger")
                .WithCronSchedule(cronConfig.NavidromeSmartPlaylistRefreshCron));
        }
        return config;
    }
}