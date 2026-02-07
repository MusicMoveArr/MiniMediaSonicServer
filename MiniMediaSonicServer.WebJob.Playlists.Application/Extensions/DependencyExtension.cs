using Microsoft.Extensions.DependencyInjection;
using MiniMediaSonicServer.WebJob.Playlists.Application.Repositories;
using MiniMediaSonicServer.WebJob.Playlists.Application.Services;

namespace MiniMediaSonicServer.WebJob.Playlists.Application.Extensions;

public static class DependencyExtension
{
    public static IServiceCollection AddPlaylistDependencies(this IServiceCollection services) =>
        services.AddScoped<PlaylistImportService>()
            .AddScoped<PlaylistImportRepository>();
}