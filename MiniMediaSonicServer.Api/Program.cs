using System.Text.Json.Serialization;
using DbUp;
using FluentValidation;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Api.Binders;
using MiniMediaSonicServer.Api.Certificates;
using MiniMediaSonicServer.Api.Filters;
using MiniMediaSonicServer.Api.Validators;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Application.Interfaces;
using MiniMediaSonicServer.Application.Repositories;
using MiniMediaSonicServer.Application.Services;
using MiniMediaSonicServer.WebJob.AutoLike.Application.Extensions;
using MiniMediaSonicServer.WebJob.Import.Application.Extensions;
using MiniMediaSonicServer.WebJob.Indexing.Application.Extensions;
using MiniMediaSonicServer.WebJob.Playlists.Application.Extensions;
using Quartz;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using StackExchange.Redis;
using MiniMediaSonicServer.WebJob.Scrobbler.Application.Extensions;

namespace MiniMediaSonicServer.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        DeployChanges.To
            .PostgresqlDatabase(builder.Configuration.GetSection("DatabaseConfiguration")["ConnectionString"])
            .WithExecutionTimeout(TimeSpan.FromMinutes(15))
            .WithScriptsFromFileSystem("./DbScripts")
            .LogToConsole()
            .Build()
            .PerformUpgrade();

        var tlsConfiguration = new TlsConfiguration();
        builder.Configuration.Bind("tls", tlsConfiguration);
        builder.Services.AddSingleton<ICertificateReader>(x =>
        {
            if (tlsConfiguration.HasCertificate)
            {
                return new CachingMountedCertificateReader(new MountedCertificateReader(tlsConfiguration));
            }
            else
            {
                return new NullCertificateReader();
            }
        });

        builder.WebHost.UseKestrel((_, options) =>
        {
            options.ListenAnyIP(8080, listenOptions => { listenOptions.Protocols = HttpProtocols.Http1; });
            options.ListenAnyIP(8081, listenOptions =>
            {
                var certificateReader = listenOptions.ApplicationServices.GetRequiredService<ICertificateReader>();
                if (tlsConfiguration.HasCertificate)
                {
                    listenOptions.Protocols = HttpProtocols.Http2 | HttpProtocols.Http3;
                    listenOptions.UseHttps(httpsOptions =>
                    {
                        httpsOptions.ServerCertificateSelector = (_, _) => certificateReader.Read();
                    });
                }
                else
                {
                    listenOptions.Protocols = HttpProtocols.Http1;
                }
            });
        });

        builder
            .Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            })
            .AddControllers(options =>
            {
                options.ModelValidatorProviders.Clear();
                options.Filters.Add(typeof(SubsonicAuthFilter));
                options.Filters.Add(typeof(ApiLoggingFilter));
                options.ModelBinderProviders.Insert(0, new HybridBinderProvider());
            })
            .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); })
            .Services.AddQuartz((q, serviceCollection) =>
            {
                var cronConfig = serviceCollection.GetRequiredService<IOptions<CronConfiguration>>().Value;

                q.AddAutoLikeJobs(cronConfig);
                q.AddPlaylistJobs(cronConfig);
                q.AddIndexingJobs(cronConfig);
                q.AddImportJobs(cronConfig);
                q.AddScrobblerJobs(cronConfig);
                q.UsePersistentStore(options =>
                {
                    options.UsePostgres(builder.Configuration.GetSection("DatabaseConfiguration")["ConnectionString"]);
                    options.UseNewtonsoftJsonSerializer();
                });
            })
            .AddQuartzHostedService()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddFluentValidationAutoValidation()
            .AddValidatorsFromAssemblyContaining<GetAlbumList2RequestValidator>()
            .Configure<DatabaseConfiguration>(builder.Configuration.GetSection("DatabaseConfiguration"))
            .Configure<EncryptionKeysConfiguration>(builder.Configuration.GetSection("EncryptionKeys"))
            .Configure<RedisConfiguration>(builder.Configuration.GetSection("Redis"))
            .Configure<ShareConfiguration>(builder.Configuration.GetSection("Shares"))
            .Configure<MusicCacheConfiguration>(builder.Configuration.GetSection("MusicCache"))
            .Configure<GlobalConfiguration>(builder.Configuration.GetSection("GlobalConfiguration"))
            .Configure<CronConfiguration>(builder.Configuration.GetSection("Jobs"));

        string redisConnectionString = builder.Configuration.GetSection("Redis")["ConnectionString"];
        if (!string.IsNullOrWhiteSpace(redisConnectionString))
        {
            builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
            builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();
        }
        else
        {
            builder.Services.AddSingleton<IRedisCacheService, RedisCacheDisabledService>();
        }

        //repositories
        builder.Services.AddScoped<BookmarkRepository>();
        builder.Services.AddScoped<UserRepository>();
        builder.Services.AddScoped<AlbumRepository>();
        builder.Services.AddScoped<StreamRepository>();
        builder.Services.AddScoped<TrackCoverRepository>();
        builder.Services.AddScoped<ArtistRepository>();
        builder.Services.AddScoped<TrackRepository>();
        builder.Services.AddScoped<PlaylistRepository>();
        builder.Services.AddScoped<SearchRepository>();
        builder.Services.AddScoped<SearchSyncRepository>();
        builder.Services.AddScoped<RatingRepository>();
        builder.Services.AddScoped<UserPlayHistoryRepository>();
        builder.Services.AddScoped<UserPropertyRepository>();
        builder.Services.AddScoped<UserPlayQueueRepository>();
        builder.Services.AddScoped<ShareRepository>();

        //services
        builder.Services.AddScoped<BookmarkService>();
        builder.Services.AddScoped<AlbumService>();
        builder.Services.AddScoped<StreamService>();
        builder.Services.AddScoped<CoverService>();
        builder.Services.AddScoped<ArtistService>();
        builder.Services.AddScoped<TrackService>();
        builder.Services.AddScoped<PlaylistService>();
        builder.Services.AddScoped<SearchService>();
        builder.Services.AddScoped<RatingService>();
        builder.Services.AddScoped<ScrobbleService>();
        builder.Services.AddScoped<ShareService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<UserPlayQueueService>();
        builder.Services.AddScoped<TranscodeService>();
        builder.Services.AddScoped<MusicCacheService>();
        builder.Services.AddScoped<SubsonicAuthFilter>();
        builder.Services.AddScoped<ApiLoggingFilter>();

        //PlaylistWebjob
        builder.Services.AddPlaylistDependencies();
        //IndexingWebJob
        builder.Services.AddIndexingDependencies();
        //ImportLastFmWebJob
        builder.Services.AddImportDependencies();
        //AutoLikeJobs
        builder.Services.AddAutoLikeDependencies();
        //AutoLikeJobs
        builder.Services.AddScrobblerDependencies();


        var app = builder.Build();

        app.UseCors(builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();

        app.Use(next => context =>
        {
            context.Request.EnableBuffering();
            return next(context);
        });

        app.UseEndpoints(endpoints => endpoints.MapControllers());

        using var scope = app.Services.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<UserService>();
        await userService.CreateFirstUserAsync();
        

        app.Run();
    }
}