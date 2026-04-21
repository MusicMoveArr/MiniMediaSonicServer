using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MiniMediaSonicServer.WebJob.AutoLike.Application.Jobs;
using MiniMediaSonicServer.WebJob.AutoLike.Application.Repositories;
using MiniMediaSonicServer.WebJob.AutoLike.Application.Services;
using Quartz;

namespace MiniMediaSonicServer.WebJob.AutoLike.Application.Extensions;

public static class DependencyExtension
{
    public static IServiceCollection AddAutoLikeDependencies(this IServiceCollection services) =>
        services.AddScoped<AutoLikeService>()
            .AddScoped<AutoRateAlbumsService>()
            .AddScoped<FingerPrintService>()
            .AddScoped<AutoRateDuplicateTracksService>()
            .AddScoped<AutoLikeArtistsJob>()
            .AddScoped<AutoLikeArtistRepository>()
            .AddScoped<AutoRateAlbumsRepository>()
            .AddScoped<AutoRateDuplicateTracksRepository>();
    
    public static IServiceCollectionQuartzConfigurator AddAutoLikeJobs(
        this IServiceCollectionQuartzConfigurator config,
        WebApplicationBuilder builder)
    {
        var jobKey = new JobKey(nameof(AutoLikeArtistsJob));
        config.AddJob<AutoLikeArtistsJob>(opts => opts.WithIdentity(jobKey));
        config.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity($"{nameof(AutoLikeArtistsJob)}-trigger")
            .WithCronSchedule(builder.Configuration.GetSection("Jobs")["AutoLikeCron"]));
        
        var albumsJobKey = new JobKey(nameof(AutoRateAlbumsJob));
        config.AddJob<AutoRateAlbumsJob>(opts => opts.WithIdentity(albumsJobKey));
        config.AddTrigger(opts => opts
            .ForJob(albumsJobKey)
            .WithIdentity($"{nameof(AutoRateAlbumsJob)}-trigger")
            .WithCronSchedule(builder.Configuration.GetSection("Jobs")["AutoLikeCron"]));
        
        var duplicateTracksJobKey = new JobKey(nameof(AutoRateDuplicateTracksJob));
        config.AddJob<AutoRateDuplicateTracksJob>(opts => opts.WithIdentity(duplicateTracksJobKey));
        config.AddTrigger(opts => opts
            .ForJob(duplicateTracksJobKey)
            .WithIdentity($"{nameof(AutoRateDuplicateTracksJob)}-trigger")
            .WithCronSchedule(builder.Configuration.GetSection("Jobs")["AutoLikeCron"]));

        return config;
    }
}