using System.Text.Json.Serialization;
using DbUp;
using FluentValidation;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Api.Filters;
using MiniMediaSonicServer.Api.Validators;
using MiniMediaSonicServer.Application.Handlers.Scrobblers;
using MiniMediaSonicServer.Application.Interfaces;
using MiniMediaSonicServer.Application.Repositories;
using MiniMediaSonicServer.Application.Services;
using MiniMediaSonicServer.WebJob.Playlists.Application.Extensions;
using MiniMediaSonicServer.WebJob.Playlists.Application.Jobs;
using Quartz;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
builder.Services.AddControllers(options =>
    {
        options.ModelValidatorProviders.Clear();
        options.Filters.Add(typeof(SubsonicAuthFilter));
        options.Filters.Add(typeof(ApiLoggingFilter));
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("PlaylistImportJob");

    q.AddJob<PlaylistImportJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("PlaylistImportJob-trigger")
        .WithCronSchedule(builder.Configuration.GetSection("Jobs")["PlaylistImportCron"]));
});

builder.Services.AddQuartzHostedService();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddValidatorsFromAssemblyContaining<GetAlbumList2RequestValidator>();

builder.Services.Configure<DatabaseConfiguration>(builder.Configuration.GetSection("DatabaseConfiguration"));
builder.Services.Configure<EncryptionKeysConfiguration>(builder.Configuration.GetSection("EncryptionKeys"));

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

//services
builder.Services.AddScoped<AlbumService>();
builder.Services.AddScoped<StreamService>();
builder.Services.AddScoped<CoverService>();
builder.Services.AddScoped<ArtistService>();
builder.Services.AddScoped<TrackService>();
builder.Services.AddScoped<PlaylistService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<RatingService>();
builder.Services.AddScoped<ScrobbleService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TranscodeService>();

//handlers
builder.Services.AddScoped<ListenBrainzScrobbleHandler>();
builder.Services.AddScoped<MalojaScrobbleHandler>();


builder.Services.AddScoped<SubsonicAuthFilter>();
builder.Services.AddScoped<ApiLoggingFilter>();

//PlaylistWebjob
builder.Services.AddPlaylistDependencies();


var app = builder.Build();

DeployChanges.To
    .PostgresqlDatabase(app.Services.GetRequiredService<IOptions<DatabaseConfiguration>>().Value.ConnectionString)
    .WithExecutionTimeout(TimeSpan.FromMinutes(15))
    .WithScriptsFromFileSystem("./DbScripts")
    .LogToConsole()
    .Build()
    .PerformUpgrade();

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();
