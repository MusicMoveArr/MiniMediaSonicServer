using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
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
            .AddScoped<FixMissingPlayHistoryTracksService>();

    public static IServiceCollectionQuartzConfigurator AddIndexingJobs(
        this IServiceCollectionQuartzConfigurator config,
        WebApplicationBuilder builder)
    {
        var jobKey = new JobKey("ReIndexSearchJob");
        config.AddJob<ReIndexSearchJob>(opts => opts.WithIdentity(jobKey));

        config.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity("ReIndexSearchJob-trigger")
            .WithCronSchedule(builder.Configuration.GetSection("Jobs")["ReIndexSearchCron"]));
        
        var jobKeyPlayHistory = new JobKey("FixMissingPlayHistoryTracksJob");
        config.AddJob<FixMissingPlayHistoryTracksJob>(opts => opts.WithIdentity(jobKeyPlayHistory));
        config.AddTrigger(opts => opts
            .ForJob(jobKeyPlayHistory)
            .WithIdentity("FixMissingPlayHistoryTracksJob-trigger")
            .WithCronSchedule(builder.Configuration.GetSection("Jobs")["PlayHistoryFixTracksCron"]));
        return config;
    }
}