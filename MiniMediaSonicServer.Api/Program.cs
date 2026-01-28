using System.Text.Json.Serialization;
using DbUp;
using FluentValidation;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Api.Filters;
using MiniMediaSonicServer.Api.Validators;
using MiniMediaSonicServer.Application.Repositories;
using MiniMediaSonicServer.Application.Services;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

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
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddValidatorsFromAssemblyContaining<GetAlbumList2RequestValidator>();

builder.Services.Configure<DatabaseConfiguration>(builder.Configuration.GetSection("DatabaseConfiguration"));
builder.Services.Configure<EncryptionKeysConfiguration>(builder.Configuration.GetSection("EncryptionKeys"));

//repositories
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<AlbumRepository>();
builder.Services.AddScoped<StreamRepository>();
builder.Services.AddScoped<TrackCoverRepository>();
builder.Services.AddScoped<ArtistRepository>();
builder.Services.AddScoped<TrackRepository>();
builder.Services.AddScoped<PlaylistRepository>();
builder.Services.AddScoped<SearchRepository>();

//services
builder.Services.AddScoped<AlbumService>();
builder.Services.AddScoped<StreamService>();
builder.Services.AddScoped<CoverService>();
builder.Services.AddScoped<ArtistService>();
builder.Services.AddScoped<TrackService>();
builder.Services.AddScoped<PlaylistService>();
builder.Services.AddScoped<SearchService>();

builder.Services.AddScoped<SubsonicAuthFilter>();

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
