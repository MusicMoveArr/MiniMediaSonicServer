using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.WebJob.Indexing.Application.Jobs;
using MiniMediaSonicServer.WebJob.Indexing.Application.Repositories;
using MiniMediaSonicServer.WebJob.Indexing.Application.Services;
using Quartz;

namespace MiniMediaSonicServer.WebJob.Indexing.Application.Extensions;

public static class DependencyExtension
{
    public static IServiceCollection AddIndexingDependencies(this IServiceCollection services) =>
        services.AddScoped<ReIndexSearchService>()
            .AddScoped<IndexedSearchRepository>()
            .AddScoped<FixMissingPlayHistoryTracksRepository>()
            .AddScoped<FixMissingRatedTracksRepository>()
            .AddScoped<FixMissingPlayHistoryTracksService>()
            .AddScoped<FixMissingRatedTracksService>();

    public static IServiceCollectionQuartzConfigurator AddIndexingJobs(
        this IServiceCollectionQuartzConfigurator config,
        CronConfiguration cronConfig)
    {
        if (!string.IsNullOrWhiteSpace(cronConfig.ReIndexSearchCron))
        {
            var jobKey = new JobKey("ReIndexSearchJob");
            config.AddJob<ReIndexSearchJob>(opts => opts.WithIdentity(jobKey));
            config.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("ReIndexSearchJob-trigger")
                .WithCronSchedule(cronConfig.ReIndexSearchCron));
        }

        if (!string.IsNullOrWhiteSpace(cronConfig.PlayHistoryFixTracksCron))
        {
            var jobKeyPlayHistory = new JobKey("FixMissingPlayHistoryTracksJob");
            config.AddJob<FixMissingPlayHistoryTracksJob>(opts => opts.WithIdentity(jobKeyPlayHistory));
            config.AddTrigger(opts => opts
                .ForJob(jobKeyPlayHistory)
                .WithIdentity("FixMissingPlayHistoryTracksJob-trigger")
                .WithCronSchedule(cronConfig.PlayHistoryFixTracksCron));
        }

        if (!string.IsNullOrWhiteSpace(cronConfig.PlayHistoryFixTracksCron))
        {
            var jobKeyRatedTracks = new JobKey("FixMissingRatedTracksJob");
            config.AddJob<FixMissingRatedTracksJob>(opts => opts.WithIdentity(jobKeyRatedTracks));
            config.AddTrigger(opts => opts
                .ForJob(jobKeyRatedTracks)
                .WithIdentity("FixMissingRatedTracksJob-trigger")
                .WithCronSchedule(cronConfig.PlayHistoryFixTracksCron));
        }
        return config;
    }
}