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
            .AddScoped<AutoLikeArtistsJob>()
            .AddScoped<AutoLikeArtistRepository>();
    
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
        
        return config;
    }
}