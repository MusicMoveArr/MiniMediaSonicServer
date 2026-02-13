using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MiniMediaSonicServer.WebJob.Import.Application.Jobs;
using MiniMediaSonicServer.WebJob.Import.Application.Services;
using Quartz;

namespace MiniMediaSonicServer.WebJob.Import.Application.Extensions;

public static class DependencyExtension
{
    public static IServiceCollection AddImportDependencies(this IServiceCollection services) =>
        services.AddScoped<ImportLastFmScrobblesJob>()
                .AddScoped<ImportLastFmScrobblesService>();
    
    public static IServiceCollectionQuartzConfigurator AddImportJobs(
        this IServiceCollectionQuartzConfigurator config,
        WebApplicationBuilder builder)
    {
        var jobKey = new JobKey("ImportLastFmScrobblesJob");
        config.AddJob<ImportLastFmScrobblesJob>(opts => opts.WithIdentity(jobKey));
        config.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity("ImportLastFmScrobblesJob-trigger")
            .WithCronSchedule(builder.Configuration.GetSection("Jobs")["ImportLastFmScrobblesCron"]));
        
        
        return config;
    }
}